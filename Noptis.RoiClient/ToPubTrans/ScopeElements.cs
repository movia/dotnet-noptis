using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Noptis.RoiClient.ToPubTrans
{
    public class ScopeElements : MessageBase, IList<ScopeElement>
    {
        private List<ScopeElement> scopeElements;

        public ScopeElements()
        {
            scopeElements = new List<ScopeElement>();
        }

        public ScopeElements(IEnumerable<ScopeElement> scopeElements)
        {
            scopeElements = new List<ScopeElement>(scopeElements);
        }

        public ScopeElement this[int index] { get => scopeElements[index]; set => scopeElements[index] = value; }

        public int Count => scopeElements.Count;

        public bool IsReadOnly => false;

        public void Add(ScopeElement item) => scopeElements.Add(item);

        public void Clear() => scopeElements.Clear();
        public bool Contains(ScopeElement item) => scopeElements.Contains(item);

        public void CopyTo(ScopeElement[] array, int arrayIndex) => scopeElements.CopyTo(array, arrayIndex);

        public IEnumerator<ScopeElement> GetEnumerator() => scopeElements.GetEnumerator();

        public int IndexOf(ScopeElement item) => scopeElements.IndexOf(item);

        public void Insert(int index, ScopeElement item) => scopeElements.Insert(index, item);

        public bool Remove(ScopeElement item) => scopeElements.Remove(item);

        public void RemoveAt(int index) => scopeElements.RemoveAt(index);

        public void ForEach(Action<ScopeElement> action) => scopeElements.ForEach(action);

        public override void WriteXmlElements(XmlWriter xmlWriter)
        {
            foreach (var scopeElement in scopeElements)
                scopeElement.WriteXml(xmlWriter);
        }

        IEnumerator IEnumerable.GetEnumerator() => scopeElements.GetEnumerator();
    }
}
