namespace RobotPainter.Calculations.Clustering
{
    public interface IClusterer<T>
    {
        public List<T> FindClusters(List<T> data, int n_clusters);
    }
}
