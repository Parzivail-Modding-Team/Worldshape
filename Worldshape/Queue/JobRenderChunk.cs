using Worldshape.Graphics;
using Worldshape.World;

namespace Worldshape.Queue
{
    class JobRenderChunk : IJob
    {
        private readonly Chunk _chunk;

        public JobRenderChunk(Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Execute(RenderManager renderManager)
        {
            _chunk.Render();
        }

        public bool CanExecuteInBackground()
        {
            return false;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
