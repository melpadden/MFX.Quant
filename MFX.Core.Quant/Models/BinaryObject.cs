namespace MFX.Core.Quant.Models
{
    public class BinaryObject<T1, T2>
    {
        public BinaryObject(T1 key1, T2 key2)
        {
            Key1 = key1;
            Key2 = key2;
        }

        public T1 Key1 { get; set; }
        public T2 Key2 { get; set; }

        public override int GetHashCode()
        {
            if (Key1 == null && Key2 == null) return 0;
            if (Key1 == null) return Key2.GetHashCode();
            if (Key2 == null) return Key1.GetHashCode();
            return Key1.GetHashCode() * Key2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return GetHashCode().Equals(obj.GetHashCode());
        }
    }
}