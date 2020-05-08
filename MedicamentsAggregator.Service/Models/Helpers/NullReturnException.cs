using System;

namespace MedicamentsAggregator.Service.Models.Helpers
{
    public class NullReturnException : Exception
    {
        public NullReturnException() {}
        public NullReturnException(string message) : base(message){}
    }
}