using BlackCoat;
using BlackCoat.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayWhat
{
    internal class GameOverMenu : UIContainer
    {
        public GameOverMenu(Core core, Input input, params UIComponent[] components) : base(core, components)
        {
            Name = nameof(GameOverMenu);

            Input = new UIInput(input, true);
            //BackgroundColor = Color.Red;
        }
    }
}
