using Microsoft.Extensions.Logging;
using ProjectInformatics.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectInformatics.Logging
{
    public static class Extensions
    {
        public static string GetDescription(this Exception e)
        {
            var builder = new StringBuilder();
            AddException(builder, e);
            return builder.ToString();
        }

        private static void AddException(StringBuilder builder, Exception e)
        {
            builder.AppendLine($"Message: {e.Message}");
            builder.AppendLine($"Stack Trace: {e.StackTrace}");

            if (e.InnerException != null)
            {
                builder.AppendLine("Inner Exception");
                AddException(builder, e.InnerException);
            }
        }

    }

    public class LoggingAdvice<T> : DispatchProxy
    {
        private T _decorated;
        private Action<string> _logInfo;
        private Action<string> _logError;

        private Func<object, string> _serializeFunction;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod != null)
            {
                try
                {
                    try
                    {
                        LogBefore(targetMethod, args);
                    }
                    catch (Exception ex)
                    {
                        //Do not stop method execution if exception
                        LogException(ex);
                    }

                    var result = targetMethod.Invoke(_decorated, args);
                    var resultTask = result as Task;

                    if (resultTask != null)
                    {
                        resultTask.ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                LogException(task.Exception.InnerException ?? task.Exception, targetMethod);
                            }
                            else
                            {
                                object taskResult = null;
                                if (task.GetType().GetTypeInfo().IsGenericType &&
                                    task.GetType().GetGenericTypeDefinition() == typeof(Task<>))
                                {
                                    var property = task.GetType().GetTypeInfo().GetProperties()
                                        .FirstOrDefault(p => p.Name == "Result");
                                    if (property != null)
                                    {
                                        taskResult = property.GetValue(task);
                                    }
                                }
                                LogAfter(targetMethod, args, taskResult);
                            }
                        });
                    }
                    else
                    {
                        try
                        {
                            LogAfter(targetMethod, args, result);
                        }
                        catch (Exception ex)
                        {
                            //Do not stop method execution if exception
                            LogException(ex);
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                    {
                        LogException(ex.InnerException ?? ex, targetMethod);
                        throw ex.InnerException ?? ex;
                    }
                }
            }
            throw new ArgumentException(nameof(targetMethod));
        }
        public static T Create(T decorated, Action<string> logInfo, Action<string> logError,
            Func<object, string> serializeFunction)
        {
            object proxy = Create<T, LoggingAdvice<T>>();
            ((LoggingAdvice<T>)proxy).SetParameters(decorated, logInfo, logError, serializeFunction);
            return (T)proxy;
        }

        private void SetParameters(T decorated, Action<string> logInfo, Action<string> logError,
            Func<object, string> serializeFunction)
        {
            if (decorated == null)
            {
                throw new ArgumentNullException(nameof(decorated));
            }
            _decorated = decorated;
            _logInfo = logInfo;
            _logError = logError;
            _serializeFunction = serializeFunction;
        }

        private string GetStringValue(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            if (obj.GetType().GetTypeInfo().IsPrimitive || obj.GetType().GetTypeInfo().IsEnum || obj is string)
            {
                return obj.ToString();
            }
            try
            {
                return _serializeFunction?.Invoke(obj) ?? obj.ToString();
            }
            catch
            {
                return obj.ToString();
            }
        }

        private void LogException(Exception exception, MethodInfo methodInfo = null)
        {
            try
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Class {_decorated.GetType().FullName}");
                errorMessage.AppendLine($"Method {methodInfo?.Name} threw exception");
                errorMessage.AppendLine(exception.GetDescription());
                _logError?.Invoke(errorMessage.ToString());
            }
            catch (Exception)
            {
                // ignored
                //Method should return original exception
            }
        }

        private void LogAfter(MethodInfo methodInfo, object[] args, object result)
        {
            var afterMessage = new StringBuilder();
            afterMessage.AppendLine($"Class {_decorated.GetType().FullName}");
            afterMessage.AppendLine($"Method {methodInfo.Name} executed");
            afterMessage.AppendLine("Output:");
            afterMessage.AppendLine(GetStringValue(result));
            var parameters = methodInfo.GetParameters();
            if (parameters.Any())
            {
                afterMessage.AppendLine("Parameters:");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var arg = args[i];
                    afterMessage.AppendLine($"{parameter.Name}:{GetStringValue(arg)}");
                }
            }
            _logInfo?.Invoke(afterMessage.ToString());
        }

        private void LogBefore(MethodInfo methodInfo, object[] args)
        {
            var beforeMessage = new StringBuilder();
            beforeMessage.AppendLine($"Class {_decorated.GetType().FullName}");
            beforeMessage.AppendLine($"Method {methodInfo.Name} executing");
            var parameters = methodInfo.GetParameters();
            if (parameters.Any())
            {
                beforeMessage.AppendLine("Parameters:");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var arg = args[i];
                    beforeMessage.AppendLine($"{parameter.Name}:{GetStringValue(arg)}");
                }
            }
            _logInfo?.Invoke(beforeMessage.ToString());
        }

        public static T Create(T decorated, ILogger logger)
        {
            return Create(
                decorated,
                s => logger.LogInformation("Info:" + s),
                s => logger.LogInformation("Error:" + s),
                o => Serialize(o));
        }

        private static string Serialize(object obj)
        {
            if (obj == null) return "null";
            if (obj is IEnumerable<object> list)
                return list.Aggregate("", (a, b) => Serialize(a) + Serialize(b));
            if (obj is Message message)
                return "Message { " + "Sender: " + message.UserName + ", Receiver: " + message.SendTo
                    + ", Text: " + message.MessageText + " }\n";
            if (obj is User user)
                return "User { " + "Email: " + user.Email + "Role: " + user.Role?.Name
                    + "Category: " + user.CategoryId + " }\n";
            return obj.ToString();
        }
    }
}
