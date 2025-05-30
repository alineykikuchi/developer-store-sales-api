using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public Money(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            return new Money(Amount * multiplier, Currency);
        }

        public Money ApplyDiscount(decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100");

            var discountAmount = Amount * (discountPercentage / 100);
            return new Money(Amount - discountAmount, Currency);
        }

        public static Money Zero(string currency = "BRL") => new Money(0, currency);
    }
}
