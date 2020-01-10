using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowing {
	class Program {
		static void Main(string[] args) {
			/*using (Game game = new Game(800, 600, "Windowing")){
				game.Run(60.0f);
			}*/
			using (PresentationGame game = new PresentationGame(800, 600, "Windowing")) {
				game.Run(60.0f);
			}
		}
	}
}
