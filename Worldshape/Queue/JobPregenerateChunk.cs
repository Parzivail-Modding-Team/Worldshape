using System.Threading.Tasks;
using MinecraftStructureLib.Core;
using Worldshape.Graphics;
using Worldshape.Graphics.Texture;

namespace Worldshape.Queue
{
    class JobPregenerateChunks : IJob
    {
        private readonly Structure _structure;
        private readonly TextureAtlas _texAtlas;

        public JobPregenerateChunks(Structure structure, TextureAtlas texAtlas)
        {
            _structure = structure;
            _texAtlas = texAtlas;
        }

        public void Execute(RenderManager renderManager)
        {
            Parallel.ForEach(renderManager.Chunks, chunk =>
            {
                chunk.Prerender(_structure, _texAtlas);
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
