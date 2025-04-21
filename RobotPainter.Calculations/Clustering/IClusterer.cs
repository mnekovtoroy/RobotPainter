using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPainter.Calculations.Clustering
{
    public interface IClusterer<T>
    {
        public List<T> FindClusters(List<T> data, int n_clusters);
    }
}
