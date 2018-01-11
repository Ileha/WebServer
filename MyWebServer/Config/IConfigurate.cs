using System.Xml.Linq;

namespace Config
{
	public interface IConfigurate
	{
		string ConfigName{ get; }
		void Configurate(XElement data);
	}
}
