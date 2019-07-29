using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpperComputer2.Core
{
    class mypoint
    {
        private float x;
        private float y;


        public mypoint(int v1, int v2)
        {
            this.x = v1;
            this.y = v2;
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        
    }
    
}
