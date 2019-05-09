using Worldshape.Graphics;

namespace Worldshape.Queue
{
    public interface IJob
    {
        /// <summary>
        /// Executes the job with the specified render context
        /// </summary>
        /// <param name="renderEngine"></param>
        void Execute(RenderEngine renderEngine);

        /// <summary>
        /// Returns true if the job doesn't need to be a part of the UI thread
        /// </summary>
        /// <returns></returns>
        bool CanExecuteInBackground();

        /// <summary>
        /// True if the job can be prematurely removed from the queue
        /// </summary>
        /// <returns></returns>
        bool IsCancellable();
    }
}
