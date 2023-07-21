using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSFramework
{
    [Serializable]
    public abstract class ApplicationCommand
    {
        protected ApplicationCommand(CommandType commandType)
        {
            this.CommandId = Guid.NewGuid().ToString();
            this.TypeOfCommand = commandType;
            this.CommandInitiationTimeUtc = DateTime.UtcNow;
        }
        public int Sequence { get; set; }
        public string CommandId { get; set; }
        public CommandType TypeOfCommand { get; set; }
        public DateTime CommandInitiationTimeUtc { get; set; }      
    }
}
