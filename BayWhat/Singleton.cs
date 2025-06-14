using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class Singleton<T>
    {
		private static T _instance;

		public static T Instance
		{
			get 
			{ 
				return _instance; 
			}
		}

		private static void Initialize()
		{

		}

    }
}
