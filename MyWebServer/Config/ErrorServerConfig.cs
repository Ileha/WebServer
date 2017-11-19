using System;
namespace Config {
    public class ErrorServerConfig : Exception {
        private string fiel_value;

        public ErrorServerConfig(string fiel_val) {
            fiel_value = fiel_val;
        }

        public override string ToString() {
            return string.Format("fiel {0} does not exist", fiel_value);
        }
    }
}
