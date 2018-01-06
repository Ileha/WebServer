using System;
using System.Collections.Generic;
using Host.HeaderData;

namespace Host.DataInput
{
	public class FileData : ABSReqestData
	{
		private Dictionary<string, HeaderValueMain> _settings;
		private string _name;
		private byte[] _data;

		public FileData(string _name, byte[] _data) {
            this._data = _data;
			this._name = _name;
		}
		public FileData(string _name, byte[] _data, params HeaderValueMain[] settings)
		{
			this._data = _data;
			this._name = _name;
			_settings = new Dictionary<string, HeaderValueMain>();
			for (int i = 0; i < settings.Length; i++) {
				if (settings[i] == null) { continue; }
				_settings.Add(settings[i].Name, settings[i]);
			}
		}
		public FileData(string _name, byte[] _data, Dictionary<string, HeaderValueMain> settings)
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
		public override byte[] ToByteArray() {
			return _data;
		}
		public void SetData(byte[] _data) {
			this._data = _data;
		}
	}
}
