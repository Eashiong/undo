

//参考自 https://pub.dev/packages/undo

using System.Collections.Generic;
using System;
namespace Undo
{
    public class ChangeStack<T>
    {
        // Limit changes to store in the history
        private uint limit;

        /// <summary>
        /// Changes to keep track of
        /// </summary>
        public ChangeStack(uint limit) => this.limit = limit;


        private readonly LinkedList<List<Change<T>>> history = new LinkedList<List<Change<T>>>();
        private readonly LinkedList<List<Change<T>>> redos = new LinkedList<List<Change<T>>>();

        /// <summary>
        /// Can redo the previous change
        /// </summary>
        public bool CanRedo => redos.Count > 0;

        /// <summary>
        /// Can undo the previous change
        /// </summary>
        public bool CanUndo => history.Count > 0;

        /// <summary>
        /// Add New Change and Clear Redo Stack
        /// </summary>
        public void Add(Change<T> change)
        {
            change.Execute();
            history.AddLast(new List<Change<T>>() { change });
            MoveForward();
        }

        private void MoveForward()
        {
            redos.Clear();
            if (limit > 0 && history.Count > limit + 1)
            {
                 history.RemoveFirst();
            }
        }
        /// <summary>
        /// Add New Group of Changes and Clear Redo Stack
        /// </summary>
        public void AddGroup(List<Change<T>> changes)
        {
            ApplyChanges(changes);
            history.AddLast(changes);
            MoveForward();
        }

        private void ApplyChanges(List<Change<T>> changes)
        {
            foreach (var change in changes)
            {
                change.Execute();
            }
        }


        /// <summary>
        /// Clear Undo History
        /// </summary> 
        public void Clear()
        {
            history.Clear();
            redos.Clear();
        }

        /// <summary>
        /// Redo Previous Undo
        /// </summary> 
        public void Redo()
        {
            if (CanRedo)
            {
                var changes = redos.First.Value;
                redos.RemoveFirst();
                ApplyChanges(changes);
                history.AddLast(changes);
            }
        }
        /// <summary>
        /// Undo Last Change
        /// </summary> 
        public void Undo()
        {
            if (CanUndo)
            {
                var changes = history.Last.Value;
                history.RemoveLast();
                foreach (var change in changes)
                {
                    change.Undo();
                }
                redos.AddFirst(changes);
            }
        }
    }

    public class Change<T>
    {
        private readonly T oldValue;
        private readonly Action execute;
        private readonly Action<T> undo;

        public Change(T oldValue, Action execute, Action<T> undo)
        {
            this.oldValue = oldValue;
            this.execute = execute;
            this.undo = undo;
        }

        public void Execute()
        {
            execute?.Invoke();
        }
        public void Undo()
        {
            undo?.Invoke(oldValue);
        }

    }
}
