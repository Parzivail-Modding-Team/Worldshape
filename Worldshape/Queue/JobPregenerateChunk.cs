using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using Worldshape.Graphics;
using Worldshape.Graphics.Texture;

namespace Worldshape.Queue
{
    class JobPregenerateChunks : IJob
    {
        private readonly Structure _structure;
        private readonly BlockAtlas _texAtlas;

        public JobPregenerateChunks(Structure structure, BlockAtlas texAtlas)
        {
            _structure = structure;
            _texAtlas = texAtlas;
        }

        public void Execute(RenderEngine renderEngine)
        {
            Parallel.ForEach(renderEngine.Chunks, chunk =>
            {
                chunk.Prerender(_structure, _texAtlas);
                renderEngine.EnqueueJob(new JobRenderChunk(chunk));
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
