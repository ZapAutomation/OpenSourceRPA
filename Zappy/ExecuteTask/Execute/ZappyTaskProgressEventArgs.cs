using System;
using Zappy.SharedInterface;

namespace Zappy.ExecuteTask.Execute
{
    public class ZappyTaskProgressEventArgs : EventArgs
    {
        private IZappyAction action;
        private int currentStep;
        private int thinkTime;
        private int totalSteps;

        public ZappyTaskProgressEventArgs(int thinkTime, IZappyAction action) : this(-1, -1, action)
        {
            this.thinkTime = thinkTime;
        }

        public ZappyTaskProgressEventArgs(int currentStep, int totalSteps, IZappyAction action)
        {
            this.totalSteps = totalSteps;
            this.currentStep = currentStep;
            this.action = action;
            thinkTime = 0;
        }

        public IZappyAction Action =>
            action;

        public int CurrentStep =>
            currentStep;

        public int ThinkTime =>
            thinkTime;

        public int TotalSteps =>
            totalSteps;
    }
}