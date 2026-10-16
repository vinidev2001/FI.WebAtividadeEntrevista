using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebAtividadeEntrevista.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CpfValidoAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var text = (value ?? string.Empty).ToString();
            if (text.Length != 11 && !Regex.IsMatch(text, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$")) return false;
            var cpf = new string(text.Where(c => c >= '0' && c <= '9').ToArray());
            if (cpf.Length != 11 || cpf.All(c => c == cpf[0])) return false;

            for (var position = 9; position < 11; position++)
            {
                var sum = 0;
                for (var i = 0; i < position; i++)
                    sum += (cpf[i] - '0') * ((position + 1) - i);
                var digit = (sum * 10) % 11;
                if (digit == 10) digit = 0;
                if (digit != cpf[position] - '0') return false;
            }
            return true;
        }
    }
}
