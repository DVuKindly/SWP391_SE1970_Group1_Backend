﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string? message = null) =>
            new ServiceResult<T> { Success = true, Message = message ?? "Success", Data = data };

        public static ServiceResult<T> Fail(string message) =>
            new ServiceResult<T> { Success = false, Message = message, Data = default };
    }
}
