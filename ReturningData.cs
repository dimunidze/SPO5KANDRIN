namespace Lab5 {
    public class ReturningData {
        public bool Contains { get; set; }
        public int Value { get; set; }
        public long Time { get; set; }
        public int Iterations { get; set; }

        public ReturningData(bool c, int v, long t) {
            Contains = c;
            Value = v;
            Time = t;
        }
        public ReturningData(bool c, int v, long t, int i) : this(c,v,t)
        {
            Iterations = i;
        }
    }
}
