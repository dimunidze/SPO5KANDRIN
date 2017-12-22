using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Lab5 {
    public class RehashMethodBasedTable {
        public struct Entry {
            //Ключ
            public string Key;
            //Значение
            public int Value;
            public Entry(string key, int value) {
                Key = key;
                Value = value;
            }
            //Операторы сравнения для структуры
            public static bool operator == (Entry current, Entry other) {
                return current.Key == other.Key && current.Value.Equals(other.Value);
            }
            public static bool operator != (Entry current, Entry other) {
                return current.Key != other.Key || !current.Value.Equals(other.Value);
            }
        }
        private static Random Rnd = new Random();
        //Элементы таблицы
        public Entry[] _entries;
        private int[] _randomNumbersSequence { get; set; }
        public RehashMethodBasedTable(int capacity) {
            _entries = new Entry[capacity];
            _randomNumbersSequence = GetRandomNumbersSequence(capacity);
        }
        //Добавление в таблицу
        public long AddValue(string key, int value) {
            var sw = new Stopwatch();
            sw.Start();
            //Вычисляем хеш ключа
            var hashCode = HashCalculator.CalculatePolynomHash(key);
            //Индекс элемента
            var index = hashCode % _entries.Length;
            //Обходим таблицу в случайном порядке (сначала попадаем в элемент с индексом, найденным выше)
            foreach (var offset in _randomNumbersSequence) {
                var newIndex = (index + offset) % _entries.Length;
                //Если эта позиция в таблице свободна - вставляем новый элемент туда.
                if (_entries[newIndex] == default(Entry)) {
                    _entries[newIndex] = new Entry(key, value);
                    sw.Stop();
                    return sw.GetNanoSeconds();
                }
            }
            //Если в таблице нет свободных мест - кидаем исключение.
            throw new Exception("В таблице не осталось свободных ячеек");
        }

        public ReturningData SearchValue(string key) {
            var sw = new Stopwatch();
            sw.Start();
            //Вычисляем хеш код и индекс
            var hashCode = HashCalculator.CalculatePolynomHash(key);
            var index = hashCode % _entries.Length;
            //Обходим таблицу в случайном порядке.
            foreach (var ind in _randomNumbersSequence) {
                var newIndex = (index + ind) % _entries.Length;
                //Если текущий элемент пустой - значит элемента с данным ключом нет в таблице.
                if (_entries[newIndex] == default(Entry)) {
                    sw.Stop();
                    return new ReturningData(false, default(int), sw.GetNanoSeconds());
                } else {
                    //Иначе, если ключтекущего элемента равен входному - элемент найден
                    if (_entries[newIndex].Key == key) {
                        sw.Stop();
                        return new ReturningData(true, _entries[newIndex].Value, sw.GetNanoSeconds());
                    }
                }
            }
            sw.Stop();
            return new ReturningData(false, default(int), sw.GetNanoSeconds());
        }
        //Генерирует случайную последовательность чисел размера, равного размеру таблицы и каждый элемент уникален.
        private int[] GetRandomNumbersSequence(int capacity) {
            HashSet<int> res = new HashSet<int>();
            int value = 0;
            res.Add(value);
            for (var i = 1; i < capacity; ++i) {
                do {
                    value = Rnd.Next(1, capacity);
                } while (res.Contains(value));
                res.Add(value);
            }
            return res.ToArray();
        }
    }
}