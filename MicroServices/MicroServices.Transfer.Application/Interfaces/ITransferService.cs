﻿using MicroServices.Transfer.Domain.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServices.Transfer.Application.Interfaces
{
    public interface ITransferService
    {
        IEnumerable<TransferLog> GetTransferLogs();
    }
}
