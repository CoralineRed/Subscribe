using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Annotations
{
    public class EmailAttribute : ValidationAttribute
    {
        private static string[] emails;

        public EmailAttribute(string[] mEmails)
        {
            emails = mEmails;
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    if (value.ToString().Contains(emails[i]))
                        return true;
                }
            }
            return false;
        }
    }
}