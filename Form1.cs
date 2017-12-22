using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Lab5 {
    public partial class Form1 : Form {
        private static Random Rnd;
        private RehashMethodBasedTable _rehashBasedTable;
        private CombinedMethodBasedTable _combinedMethodBasedTable;
        private int _capacity;
        private List<DataForAdd> _rehashMethodAddingData;
        private List<DataForAdd> _combinedMethodAddingData;
        private List<DataForSearch> _rehashMethodSearchingData;
        private List<DataForSearch> _combinedMethodSearchingData;
        private char[] _alphabet = new[]{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                                         'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                                         's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        private KeyValuePair<string, int>[] _testElements;
        private string[] _keysForSearch;
        static Form1() {
            Rnd = new Random();
        }

        public Form1() {
            InitializeComponent();
            Debug.WriteLine("ttt");
            _rehashMethodAddingData = new List<DataForAdd>();
            _combinedMethodAddingData = new List<DataForAdd>();
            _rehashMethodSearchingData = new List<DataForSearch>();
            _combinedMethodSearchingData = new List<DataForSearch>();
        }

        private KeyValuePair<string, int>[] GetRandomElements(int capacity) {
            var uniqueSet = new HashSet<string>();
            KeyValuePair<string, int>[] res = new KeyValuePair<string, int>[capacity];
            for (var i = 0; i < capacity; ++i) {
                var keyLength = Rnd.Next(1, 40);
                var key = string.Empty;
                do {
                    key = GenerateKey();
                } while (uniqueSet.Contains(key));
                uniqueSet.Add(key);
                var value = Rnd.Next(0, 10000000);
                res[i] = new KeyValuePair<string, int>(key, value);
            }
            return res;
        }

        private string GenerateKey() {
            var keyLength = Rnd.Next(1, 40);
            var key = string.Empty;
            for (var j = 0; j < keyLength; ++j) {
                key += _alphabet[Rnd.Next(_alphabet.Length)];
            }
            return key;
        }

        private string[] GetKeysForSearch(int capacity, KeyValuePair<string, int>[] elements) {
            var res = new string[capacity];
            for (var i = 0; i < elements.Length; ++i) {
                res[i] = elements[i].Key;
            }
            return res;
        }

        private async void TestAddBtn_Click(object sender, EventArgs e) {
            testSearchBtn.Enabled = false;
            _capacity = (int)numericUpDown1.Value;
            _rehashBasedTable = new RehashMethodBasedTable(_capacity);
            _combinedMethodBasedTable = new CombinedMethodBasedTable(_capacity);
            _testElements = GetRandomElements(_capacity);
            _keysForSearch = GetKeysForSearch(_capacity, _testElements);
            label1.Text = "Рехеширование. Добавление";
            label2.Text = "Комбинированный метод. Добавление";
            label3.Text = "Рехеширование. Поиск";
            label4.Text = "Комбинированный метод. Поиск";
            await Task.Run(() => {
                ClearDataSources();
                Invoke((MethodInvoker)(() => UpdateAddLogs()));
                foreach (var elem in _testElements) {
                    var rehashAddTime = _rehashBasedTable.AddValue(elem.Key, elem.Value);
                    var combinedAddTime = _combinedMethodBasedTable.Add(elem.Key, elem.Value);
                    var rehashAddData = new DataForAdd {
                        Key = elem.Key,
                        Value = elem.Value,
                        Time = rehashAddTime
                    };
                    var combinedAddData = new DataForAdd {
                        Key = elem.Key,
                        Value = elem.Value,
                        Time = combinedAddTime
                    };
                    _rehashMethodAddingData.Add(rehashAddData);
                    _combinedMethodAddingData.Add(combinedAddData);
                }
                var rehashTotalTime = _rehashMethodAddingData.Select(x => x.Time).Sum() / 1e9;
                var combinedTotalTime = _combinedMethodAddingData.Select(x => x.Time).Sum() / 1e9;
                Invoke((MethodInvoker)(() => label1.Text = $"Рехеширование. Добавление - {rehashTotalTime} c."));
                Invoke((MethodInvoker)(() => label2.Text = $"Комбинированный метод. Добавление - {combinedTotalTime} c."));
            });
            UpdateAddLogs();
            testSearchBtn.Enabled = true;
        }

        private async void TestSearchBtn_Click(object sender, EventArgs e) {
            await Task.Run(() => {
                foreach (var key in _keysForSearch) {
                    var rehashSearchRes = _rehashBasedTable.SearchValue(key);
                    var combinedSearchRes = _combinedMethodBasedTable.SearchValue(key);
                    var rehashSearchData = new DataForSearch {
                        Key = key,
                        Contais = rehashSearchRes.Contains,
                        Value = rehashSearchRes.Contains ? (int?)rehashSearchRes.Value : null,
                        Time = rehashSearchRes.Time
                    };
                    var combinedSearchData = new DataForSearch {
                        Key = key,
                        Contais = combinedSearchRes.Contains,
                        Value = combinedSearchRes.Contains ? (int?)combinedSearchRes.Value : null,
                        Time = combinedSearchRes.Time,
                        Iterations = combinedSearchRes.Iterations
                    };
                    _rehashMethodSearchingData.Add(rehashSearchData);
                    _combinedMethodSearchingData.Add(combinedSearchData);
                }
                var rehashTotalTime = _rehashMethodSearchingData.Select(x => x.Time).Sum() / 1e9;
                var combinedTotalTime = _combinedMethodSearchingData.Select(x => x.Time).Sum() / 1e9;
                Invoke((MethodInvoker)(() => label3.Text = $"Рехеширование. Поиск - {rehashTotalTime} c."));
                Invoke((MethodInvoker)(() => label4.Text = $"Комбинированный метод. Поиск - {combinedTotalTime} c."));
            });
            UpdateSearchLogs();
            testSearchBtn.Enabled = false;
        }

        private void ClearDataSources() {
            _rehashMethodAddingData.Clear();
            _rehashMethodSearchingData.Clear();
            _combinedMethodAddingData.Clear();
            _combinedMethodSearchingData.Clear();
        }

        private void UpdateAddLogs() {
            richTextBox1.Clear();
            richTextBox2.Clear();
            var rehashsb = new StringBuilder();
            var combinedsb = new StringBuilder();
            foreach(var data in _rehashMethodAddingData) {
                rehashsb.Append(data.Key + "; " + data.Value + "; " + data.Time + "нс." + Environment.NewLine);
            }
            foreach (var data in _combinedMethodAddingData) {
                combinedsb.Append(data.Key + "; " + data.Value + "; " + data.Time + "нс." + Environment.NewLine);
            }
            richTextBox1.Text = rehashsb.ToString();
            richTextBox2.Text = combinedsb.ToString();
        }

        private void UpdateSearchLogs() {
            richTextBox3.Clear();
            richTextBox4.Clear();
            var rehashsb = new StringBuilder();
            var combinedsb = new StringBuilder();
            foreach (var data in _rehashMethodSearchingData) {
                rehashsb.Append(data.Key + "; " + data.Value + "; " + (data.Contais ? "Да; " : "Нет; ") + data.Time + "нс." + "; " + data.Iterations + Environment.NewLine);
            }
            foreach (var data in _combinedMethodSearchingData) {
                combinedsb.Append(data.Key + "; " + data.Value + "; " + (data.Contais ? "Да; " : "Нет; ") + data.Time + "нс." + "; " + data.Iterations + Environment.NewLine);
            }
            richTextBox3.Text = rehashsb.ToString();
            richTextBox4.Text = combinedsb.ToString();
        }

        private void Form1_Load(object sender, EventArgs e) {
          
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}