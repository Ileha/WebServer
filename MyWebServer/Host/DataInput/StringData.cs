using System;
using Host.HeaderData;
using System.Collections.Generic;

namespace Host.DataInput
{
	public class StringData : ABSReqestData
	{
		private Dictionary<string, HeaderValueMain> _settings;
		private string _name;
		private string _data;

		public StringData(string _name, string _data) {
        	this._data = _data;
			this._name = _name;
		}
		public StringData(string _name, string _data, params HeaderValueMain[] settings)
		{
			this._data = _data;
			this._name = _name;
			_settings = new Dictionary<string, HeaderValueMain>();
			for (int i = 0; i<settings.Length; i++) {
				_settings.Add(settings[i].Name, settings[i]);
			}
		}
		public StringData(string _name, string _data, Dictionary<string, HeaderValueMain> settings)
		{
			this._data = _data;
			this._name = _name;
			_settings = settings;
		}

		public override string Name {
			set {
				_name = value;
			}
			get {
				return _name;
			}
		}
		public override Dictionary<string, HeaderValueMain> settings {
			get {
				if (_settings == null) {
					_settings = new Dictionary<string, HeaderValueMain>();
				}
				return _settings;
			}
		}
		public override string ToStr() {
			return _data;
		}
	}
}
