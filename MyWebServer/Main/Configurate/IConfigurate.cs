using System.Xml.Linq;

namespace Configurate
{
	public interface IConfigurate
	{
		string[] ConfigName{ get; }
		void Configurate(XElement data);
	}
}
