using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class Timer
    {
        public required string name;
        public int timer = 0;
        public bool paused = false;
        public bool cancelled = false;
    }
}
