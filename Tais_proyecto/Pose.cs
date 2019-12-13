using Microsoft.Kinect;
using System;

namespace Tais_proyecto
{
    class Pose
    {
        private Skeleton inputdata;
        private String outputdata;

        public Skeleton InputData
        {
            set
            {
                if (value != null)
                {
                    inputdata = value;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            get
            {
                return inputdata;
            }
        }

        public String OutputData
        {
            set
            {
                outputdata = value;
            }
            get
            {
                return outputdata;
            }
        }
    }
}
