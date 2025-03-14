//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ML.OnnxRuntime;
//using Microsoft.ML.OnnxRuntime.Tensors;
//using System.Collections.Generic;
//using System.Linq;

//namespace BudgetManBackEnd.MessageCronJob
//{
//    public class DocumentRetriever
//    {
//        private readonly List<(string Document, float[] Embedding)> _documents = new();
//        private readonly InferenceSession _session;

//        public DocumentRetriever(string modelPath)
//        {
//            _session = new InferenceSession(modelPath); // Load embedding model
//        }

//        public void AddDocument(string document, float[] embedding)
//        {
//            _documents.Add((document, embedding));
//        }

//        public IEnumerable<string> Retrieve(string query, int topK = 1)
//        {
//            var queryEmbedding = Embed(query);

//            return _documents
//                .Select(d => (Document: d.Document, Similarity: CosineSimilarity(queryEmbedding, d.Embedding)))
//                .OrderByDescending(d => d.Similarity)
//                .Take(topK)
//                .Select(d => d.Document);
//        }

//        private float[] Embed(string text)
//        {
//            var inputTensor = new DenseTensor<string>(new[] { 1 }, new[] { text });
//            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };

//            using var results = _session.Run(inputs);
//            return results.First().AsEnumerable<float>().ToArray();
//        }

//        private static float CosineSimilarity(float[] vectorA, float[] vectorB)
//        {
//            var dot = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
//            var magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
//            var magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

//            return (float)(dot / (magnitudeA * magnitudeB));
//        }
//    }
//}
