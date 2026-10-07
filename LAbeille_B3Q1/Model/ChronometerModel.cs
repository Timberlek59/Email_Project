using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAbeille_B3Q1.Model
{
    public class ChronometerModel
    {
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

        public void Reset()
        {
            ElapsedTime = TimeSpan.Zero;
        }

        public void AddSecond()
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
        }
    }
}
