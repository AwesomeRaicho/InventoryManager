using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public T? Value { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(string error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error) => new(error);
    }
}
