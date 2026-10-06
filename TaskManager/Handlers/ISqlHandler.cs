using TaskManager.Data;
using TaskManager.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandDb.Handlers;

public interface ISqlHandler
{
    Task Execute();
    Task<AlertModel> ProcessMessage(AlertModel alert, string command);
    Task<AlertModel> WorkTheAlert(AlertModel alert, string command);
}
