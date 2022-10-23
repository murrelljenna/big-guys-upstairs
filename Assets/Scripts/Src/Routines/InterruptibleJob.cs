using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.routines
{
    interface IInterruptibleJob
    {
        void Execute();
        void Interrupt();
    }

    public abstract class InterruptibleJob : IInterruptibleJob
    {
        protected UnityEvent interrupted = new UnityEvent();
        private LocalGameManager gameManager;
        private Coroutine routine;

        public void Execute()
        {
            gameManager = LocalGameManager.Get();
            routine = gameManager.StartCoroutine(execute_impl());
        }

        protected abstract IEnumerator execute_impl();
        public void Interrupt()
        {
            if (routine != null)
            {
                gameManager.StopCoroutine(routine);
            }
            interrupted.Invoke();
        }

        protected void markForCleanup(UnityEvent ev, UnityAction action)
        {
            interrupted.AddListener(() => ev.RemoveListener(action));
        }

        protected void markForCleanup<T>(UnityEvent<T> ev, UnityAction<T> action)
        {
            interrupted.AddListener(() => ev.RemoveListener(action));
        }

        ~InterruptibleJob()
        {
            Interrupt();
        }
    }
}