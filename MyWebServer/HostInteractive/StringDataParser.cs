using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostInteractive
{
    static class StringDataParser {
        public static string[] Parse(string cmd)
        {
            List<string> cmd_args = new List<string>();
            bool is_quotes = false;
            int index_last = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                char current = cmd[i];
                if (current == ' ' && is_quotes == false)
                {
                    cmd_args.Add(cmd.Substring(index_last, i - index_last));
                    index_last = i + 1;
                }
                else if (current == '"' || current == '\'')
                {
                    is_quotes = !is_quotes;
                    cmd = cmd.Remove(i, 1);
                    i--;
                }
            }
            cmd_args.Add(cmd.Substring(index_last, cmd.Length - index_last));
            return cmd_args.ToArray();
        }
        public static string[] Parse(string cmd, int parse_word_count)
        {
            List<string> cmd_args = new List<string>();
            bool is_quotes = false;
            int index_last = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                char current = cmd[i];
                if (current == ' ' && is_quotes == false)
                {
                    cmd_args.Add(cmd.Substring(index_last, i - index_last));
                    index_last = i + 1;
                    if (parse_word_count == cmd_args.Count) {
                        break;
                    }
                }
                else if (current == '"' || current == '\'')
                {
                    is_quotes = !is_quotes;
                    cmd = cmd.Remove(i, 1);
                    i--;
                }
            }
            cmd_args.Add(cmd.Substring(index_last, cmd.Length - index_last));
            return cmd_args.ToArray();
        }
        //parse_word_count слов возвращает стальные в remainder
        public static string[] Parse(string cmd, int parse_word_count, out string[] remainder)
        {
            List<string> cmd_args = new List<string>();
            List<string> cmd_remainder = new List<string>();
            bool active = true;
            Func<List<string>> get_active_array = () => {
                if (active) { return cmd_args; }
                else { return cmd_remainder; }
            };


            bool is_quotes = false;
            int index_last = 0;
            for (int i = 0; i < cmd.Length; i++)
            {
                char current = cmd[i];
                if (current == ' ' && is_quotes == false)
                {
                    get_active_array().Add(cmd.Substring(index_last, i - index_last));
                    index_last = i + 1;
                    if (parse_word_count == cmd_args.Count)
                    {
                        active = false;
                    }
                }
                else if (current == '"' || current == '\'')
                {
                    is_quotes = !is_quotes;
                    cmd = cmd.Remove(i, 1);
                    i--;
                }
            }
            get_active_array().Add(cmd.Substring(index_last, cmd.Length - index_last));
            remainder = cmd_remainder.ToArray();
            return cmd_args.ToArray();
        }
    }
}
