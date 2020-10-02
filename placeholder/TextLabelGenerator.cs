using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace f3
{
    public class TextLabelGenerator
    {
        public enum Alignment
        {
            HVCenter
        }

        public string Text;
        public float Scale;
        public Vector2 Translate;
        public Alignment Align;

        public List<fGameObject> Generate()
        {
            throw new NotImplementedException();
        }
    }
}
