using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using Worldshape.Graphics;

namespace Worldshape.Queue
{
    class JobPregenerateChunks : IJob
    {
        private readonly Structure _structure;

        public JobPregenerateChunks(Structure structure)
        {
            _structure = structure;
        }

        public void Execute(RenderManager renderManager)
        {
            Parallel.ForEach(renderManager.Chunks, chunk =>
            {
                chunk.Prerender(_structure);
                renderManager.EnqueueJob(new JobRenderChunk(chunk));
            });
        }

        public bool CanExecuteInBackground()
        {
            return true;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
