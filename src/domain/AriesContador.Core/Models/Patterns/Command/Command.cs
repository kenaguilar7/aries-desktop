using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Models.Patterns.Command
{
    public class CommandWorker
    {
        private List<IActionClass> Commands = new List<IActionClass>();

        public void SetCommand(IActionClass actionClass)
        {
            Commands.Add(actionClass);
        }

        public async Task Run()
        {
            foreach (var command in Commands)
                await command.Execute();
        }

        public void Clear()
        {
            Commands.Clear();
        }

    }

    public interface IActionClass
    {
       Task Execute();
    }

    public interface IActionResult<out T>
    {
        T Execute();
    }
}
