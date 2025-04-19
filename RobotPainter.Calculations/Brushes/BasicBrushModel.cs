using RobotPainter.Calculations.Optimization;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics;

namespace RobotPainter.Calculations.Brushes
{
    public class BasicBrushModel : IBrushModel
    {
        public readonly static string model_name = "Malevich 6";

        private readonly IOptimizer _optimizer;

        public readonly static PointD[] footprint = [
            new PointD(27.447855, 0.536619),
            new PointD(28.201554, 0.775132),
            new PointD(28.869389, 1.233075),
            new PointD(29.317792, 1.782544),
            new PointD(29.651710, 2.454259),
            new PointD(29.880682, 3.179337),
            new PointD(30.201969, 4.677195),
            new PointD(30.310004, 5.980738),
            new PointD(30.087150, 7.662308),
            new PointD(29.427442, 8.717410),
            new PointD(28.679915, 9.508738),
            new PointD(27.944634, 9.888365),
            new PointD(27.447857, 9.961784)
        ];

		public BasicBrushModel()
        {
            
        }

        public double rfun(double z)
        {
            double p1 = 0.002054;
            double p2 = -0.05006;
            double p3 = 0.3085;
            double p4 = 0.8559;
            double p5 = 3.122;
            double f = p1 * Math.Pow(z, 4) + p2 * Math.Pow(z, 3) + p3 * Math.Pow(z, 2) + p4 * z + p5;
            return f;
        }

        public double wfun(double z)
        {
            return 0.6 * z + 1.07;
        }

        public double bfun(double z)
        {
            double kb = 0.5;
            return kb * z;
        }

        public double CalculateZCoordinate(double w)
        {
			if (w < 0)
			{
				return 5;
			}
			return -(Math.Abs((w - 1.07) / 0.6));
		}

        public List<Point3D> CalculateBrushRootPath(List<Point3D> desired_path)
        {
            var desired_path_2d = desired_path.Select(p => new PointD(p.x, p.y)).ToList();
            List<PointD> result2d = [ desired_path_2d[0] ];

			var result = new List<Point3D>();
			result.Add(desired_path[0]);

            PointD p1 = desired_path_2d[0];

            for(int i = 1; i <  desired_path.Count; i++)
            {
                //disparity
                PointD q0 = result2d[i - 1];
                PointD v_d = desired_path_2d[i] - desired_path_2d[i - 1];
                PointD p0 = p1;
                PointD p1s = desired_path_2d[i];

                //normilize
                double r = rfun(-desired_path[i].z);
                PointD e = v_d / Math.Sqrt(v_d.x * v_d.x + v_d.y * v_d.y);
                PointD qstart = desired_path[i - 1].z > 0 ? p1s : p1s + r * e;

                double zcurr = desired_path[i].z;
                double zprev = desired_path[i - 1].z;

                PointD next_point = Findq(zcurr, zprev, q0, p0, p1s, qstart);
                result2d.Add(next_point);
                result.Add(new Point3D(next_point.x, next_point.y, desired_path[i].z));
            }
            return result;
        }

        private PointD Findq(double zcurr, double zprev, PointD q0, PointD p0, PointD p1, PointD q1start)
        {
            if(zcurr >= 0 && zprev < 0)
            {
                double t = -zprev / (zcurr - zprev);
                q1start = q0 * (1 - t) + t * q1start; // last point
                zcurr = 0; // last z
            }
            if(zcurr >= 0 && zprev >= 0)
            {
                return q1start;
            }
            if(zcurr < 0 && zprev >= 0)
            {
                double t = zprev / (zcurr - zprev);
                q0 = q0 * (1 - t) + t * q1start; // first point
                p0 = q0;
                zprev = 0;
            }

            double D = double.MaxValue;
            double tol = 0.2;
            int kmax = 100;
            int k = 0;
            double r1 = rfun(-zcurr);
            PointD q1 = q1start;
            while(D >= tol && k < kmax)
            {
                PointD p = Getpa(q0, q1, p0, zcurr, zprev, p0);
                PointD d = p - p1;
                double Dnew = Math.Sqrt(d.x * d.x + d.y * d.y);
                if(Dnew < D)
                {
                    var dv = q1 - p;
                    q1 = p1 + dv * r1 / Math.Sqrt(dv.x * dv.x + dv.y * dv.y);
                } else
                {
                    k = kmax;
                }
                k++;
            }

            return q1;
        }

        private PointD Getpa(PointD q0, PointD q1, PointD p0, double zcurr, double zprev, PointD pinit)
        {
            //get new point p of brush center
            PointD v = q1 - q0;
            double s = Math.Sqrt(v.x * v.x + v.y * v.y);

            PointD vq0 = q0 - pinit;
            double al0;
            if (Math.Sqrt(vq0.x * vq0.x + vq0.y * vq0.y) > 1e-16)
            {
                //al0 = Math.Acos((v.x * vq0.x + v.y * vq0.y) / (Math.Sqrt(v.x * v.x + v.y * v.y) * Math.Sqrt(vq0.x * vq0.x + vq0.y * vq0.y)));
                al0 = Geometry.CalculateAngleDeg(v, vq0) * Math.PI / 180.0; //in radians
            } else
            {
                al0 = 0.0;
            }

            al0 = Math.Abs(al0);

            if (al0 > Math.PI / 2.0)
            {
                // calculate final vector
                PointD vp = q1 - p0;
                double cosg1 = Math.Cos(Geometry.CalculateAngleDeg(v, vp) * Math.PI / 180.0); //cos between vectors
                double alp1 = Math.Acos(cosg1); //alpha

                if(alp1 < Math.PI / 2.0) //if there is a possibility to draw
                {
                    if(-rfun(-zcurr) <= Math.Sqrt(vp.x*vp.x + vp.y*vp.y)) //if in final point r <= ||vp||
                    {
                        Func<double, double> fr = t => rfun(-zprev - t * (zcurr - zprev));
                        PointD prjp0 = (vp.x * v.x + vp.y * v.y) * v / Math.Pow(s, 2);
						PointD vdel = vp - prjp0; //distance in the point closest to v
                        double vdn = Geometry.Norm(vdel);
                        double td = Geometry.Norm(p0 + vdel - q0) / s;

                        al0 = Math.PI / 2.0;
                        
                        if(vdn < fr(td)) // if in this point the distance is <= r
                        {
                            //find a point where distane is equal to r
                            Func<double, PointD> fq = t => (q0 + t * (q1 - q0));
                            Func<double, double> fopt = t => Geometry.Norm(fq(t) - p0) - fr(t);

                            double t = Optimization.Optimization.Fzero(fopt, td, 1);

                            if (t > 1)
                                t = 1;
                            if (t <= 0)
                                t = 0;

                            vp = fq(t) - p0;
							cosg1 = Math.Cos(Geometry.CalculateAngleDeg(v, vp) * Math.PI / 180.0); //cos between vectors
                            if (cosg1 > 1.0)
                                cosg1 = 1.0; //if a numerical error occurs

                            al0 = Math.Acos(cosg1); //alpha
                            zprev = zprev + t * (zcurr - zprev);

                            v = q1 - fq(t);
                            s = Geometry.Norm(v);
						} else
                        {
							//change v to the projection of (q1 - p0) onto v
							prjp0 = (vp.x * v.x + vp.y * v.y) * v / Math.Pow(s, 2);
                            v = prjp0;
                            s = Geometry.Norm(prjp0);
						}
					} else
                    {
                        //al1 = alp1
                        return p0;
                    }
                } else
                {
                    //check the case if r(zcurr) < ||vp||
                    //al1 = alp1;
                    if(rfun(-zcurr) < Geometry.Norm(vp))
                    {
                        return q1 - vp * rfun(-zcurr) / Geometry.Norm(vp);
                    } else
                    {
                        return p0;
                    }
                }

            }

            //check another case
            PointD vq = q1 - p0;
            double rd = Geometry.Norm(vq);
            double rcurr = rfun(-zcurr);
            if(rd < rcurr)
            {
                return p0;
				/*cosgnew = v'*vq/norm(v)/norm(vq); %cos between vectors
	            al1 = acos(cosgnew); % alpha
	            if cosgnew >= 1
		            al1 = 0;*/
			}

            double alp = Fal(al0, s, zcurr, zprev);
            double lat = -(rfun(-zcurr)) * Math.Cos(alp);

            int sgn2 = Math.Sign(vq0.x * v.y - vq0.y * v.x);

            double orth = sgn2 * (rfun(-zcurr)) * Math.Sin(alp);

            double cosg = v.x / s;
            double sing = -v.y / s;

            double dxpix = lat * cosg + orth * sing;
            double dypix = -lat * sing + orth * cosg;

            //al1 = alp
            return new PointD(q1.x + dxpix, q1.y + dypix);
        }

        private double Fal(double al0, double ds, double zcurr, double zprev)
        {
            Func<double, double> zfun = s => -zprev + (-zcurr + zprev) * s / ds; //with inverse z

            throw new NotImplementedException();
            double int0 = 0;
            //double int0 = integral(x => 1.0 / rfun(zfun(x)), 0, ds, 'AbsTol', 1e-13);
            return 2 * Math.Atan(Math.Tan(al0 / 2.0) * Math.Exp(-int0));
        }
    }
}
