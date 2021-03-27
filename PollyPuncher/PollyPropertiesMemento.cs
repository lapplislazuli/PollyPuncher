using System.Collections.Generic;
using System.Linq;

namespace PollyPuncher
{
    
    /**
     * This class implements the Memento Pattern and keeps a history (and history management) of PollyProperties.
     * Whenever the User presses "Play" or "Save" with a new combination of properties,
     * a PollyProperties object is stored in this memento.
     *
     * They can be navigated forward and backward in history .
     *
     * If a new Point is created while the Memento is in a position of the past,
     * all history after the current point will be lost. 
     */
    public class PollyPropertiesMemento
    {
        private int _index = -1;
        private List<PollyProperties> _history = new List<PollyProperties>();

        /*
         *
         * Returns null if no history is there (someone pressed it before ever sending anything).
         */
        public PollyProperties MoveBack()
        {
            if (_index > 0)
            {
                _index--;
                return _history[_index];
            }
            else
            {
                return _history[0];
            }
        }

        /*
         *
         * Returns null if no history is there (someone pressed it before ever sending anything).
         */
        public PollyProperties MoveForth()
        {
            if (_index < _history.Count -1)
            {
                _index++;
                return _history[_index];
            }
            else
            {
                return _history.Last();
            }
        }

        /*
         * Makes a new savepoint at index+1.
         * If the point is already known, do nothing.
         * If a new Point is created while the Memento is in a position of the past,
         * all history after the current point will be lost. 
         */
        public void MakeMemento(PollyProperties ps)
        {
            var memento = (PollyProperties) ps.Clone();
            
            // Short-Circuit: 
            // memento is already in history - do nothing, return early.
            if (_history.Contains(memento))
                return;
            
            // Case 1: Current Index is last of List 
            if (_history.Count == 0 || _index== -1 || _index == _history.Count -1 )
            {
                // "Just" add a copy of current Properties to Memento List 
                _history.Add(memento);
                // Increase Index by 1
                _index++;
            }
            // Case 2: Current Index is not last of the list 
            else
            {
                // Drop all items after index, "make current index last index"
                this._history = _history.GetRange(0, this._index);
                // Do Case 1
                _history.Add(memento);
                _index++;
            }
        }

        public bool HasElements()
        {
            return _history != null && _history.Count > 0;
        }
        
    }
}