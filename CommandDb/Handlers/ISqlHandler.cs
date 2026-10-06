using CommandDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandDb.Handlers;

public interface ISqlHandler
{
    Task<bool> Execute(AlertModel alert, string Command);
}
