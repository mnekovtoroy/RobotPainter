using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace RobotPainter.Calculations.Clustering
{
    public class KMeansClustering : IClusterer<ColorLab>
    {
        public class DataPoint
        {
            [VectorType(3)]
            public float[] Coordinates { get; set; }
        }

        public List<ColorLab> FindClusters(List<ColorLab> data, int n_clusters)
        {
            List<DataPoint> dataPoints = data.Select(c => new DataPoint { Coordinates = [Convert.ToSingle(c.L), Convert.ToSingle(c.a), Convert.ToSingle(c.b)] }).ToList();

            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(dataPoints);
            var pipeline = mlContext.Clustering.Trainers.KMeans(new KMeansTrainer.Options()
            {
                NumberOfClusters = n_clusters,
                FeatureColumnName = "Coordinates"
            });
            var model = pipeline.Fit(dataView);

            var cluster_centroids = new List<ColorLab>();

            KMeansModelParameters modelParams = model.Model;

            VBuffer<float>[] clusterCentroids = default;
            modelParams.GetClusterCentroids(ref clusterCentroids, out int k);

            for (int i = 0; i < k; i++)
            {
                var centroid = clusterCentroids[i];
                cluster_centroids.Add(new ColorLab(
                    centroid.GetItemOrDefault(0),
                    centroid.GetItemOrDefault(1),
                    centroid.GetItemOrDefault(2)
                ));
            }
            return cluster_centroids;
        }
    }
}
