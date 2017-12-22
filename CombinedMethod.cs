using System.Diagnostics;
namespace Lab5 {
    class CombinedMethodBasedTable {
        //Узел дерева
        private class Entry {
            //Ключ
            public string Key;
            //Значение
            public int Value;
            //Левый потомок
            public Entry Left;
            //Правй потомок
            public Entry Right;
            public Entry(string key, int value) {
                Key = key;
                Value = value;
            }
        }
        //Корзины
        private Entry[] _buckets;
        public CombinedMethodBasedTable(int capacity) {
            _buckets = new Entry[capacity];
        }
        public long Add(string key, int value) {
            var sw = new Stopwatch();
            sw.Start();
            var newNode = new Entry(key, value);
            //Вычисляем хеш ключа.
            var hashCode = HashCalculator.CalculatePolynomHash(key);
            //Вычисляем индекс корзины.
            var index = hashCode % _buckets.Length;
            //Если корзина пуста (т.е в таблице нет значения с таким хешем)
            if (_buckets[index] == null) {
                //Добавляем новое дерево.
                _buckets[index] = newNode;
                sw.Stop();
                return sw.GetNanoSeconds();
            // Иначе в таблице уже есть элемент с таким же хешем. Добавляем элемент в соответствующее дерево.
            } else {
                var root = _buckets[index];
                AddNode(root, newNode);
                sw.Stop();
                return sw.GetNanoSeconds();
            }
        }
        //Добавление элемента в дерево.
        private void AddNode(Entry root, Entry node) {
            //Если ключ нового узла не больше ключа корня, идем в левое поддерево, иначе в правое
            if(node.Key.CompareTo(root.Key) <= 0) {
                //Если левого потомка нет, добавляем новый элемент и выходим
                if (root.Left == null) {
                    root.Left = node;
                    return;
                } else {
                    //Иначе рекурсивный вызов, root = root.left
                    AddNode(root.Left, node);
                }
            } else {
                //Аналогично для правого потомка
                if (root.Right == null) {
                    root.Right = node;
                    return;
                } else {
                    //Иначе рекурсивный вызов, root = root.left
                    AddNode(root.Right, node);
                }
            }
        }
        public ReturningData SearchValue(string key) {
            var iterations = 0;
            var sw = new Stopwatch();
            sw.Start();
            //Вычисляем хеш ключа
            var hashCode = HashCalculator.CalculatePolynomHash(key);
            //Вычисляем индекс корзины.
            var index = hashCode % _buckets.Length;
            //Если корзина пуста - элемента нет в таблице
            if (_buckets[index] == null) {
                sw.Stop();
                return new ReturningData(false, default(int), sw.GetNanoSeconds(), 1);
            } else if (_buckets[index].Key == key) {
                //Иначе, если ключ равен ключу корневого элемента, то корневой элемент - найденное значение.
                sw.Stop();
                return new ReturningData(true, _buckets[index].Value, sw.GetNanoSeconds(), 1);
            } else {
                var root = _buckets[index];
                var res = SearchInTree(root, key, iterations);
                sw.Stop();
                res.Time = sw.GetNanoSeconds();
                return res;
            }
        }
        private ReturningData SearchInTree(Entry root, string key, int iterations) {
            //Если ключ рассматриваемого поддерева
            if (root.Key == key) {
                iterations++;
                return new ReturningData(true, root.Value, 0, iterations);
            } else {
                //Если ключ не больше ключа корня - идем в левое поддерево, иначе в правое 
                if (key.CompareTo(root.Key) <= 0) {
                    //Если левого поддерева не существует - элемента нет
                    if (root.Left == null) {
                        iterations++;
                        return new ReturningData(false, default(int), 0, iterations);
                    } else {
                        //Иначе root = root.left и вызываем рекурсивно поиск
                        iterations++;
                        return SearchInTree(root.Left, key, iterations);
                    }
                } else {
                    //Тоже самое для правого поддерева
                    if (root.Right == null) {
                        iterations++;
                        return new ReturningData(false, default(int), 0, iterations);
                    } else {
                        iterations++;
                        return SearchInTree(root.Right, key, iterations);
                    }
                }
            }
        }
    }
}