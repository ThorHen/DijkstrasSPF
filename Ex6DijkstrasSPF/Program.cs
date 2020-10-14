using System;
using System.Collections.Generic;
using System.Linq;

namespace Ex6DijkstrasSPF {
	class Program {
		static void Main(string[] args) {
			NodeWrapper<string> ohio = new NodeWrapper<string>("Ohio");
			NodeWrapper<string> nevada = new NodeWrapper<string>("Nevada");
			NodeWrapper<string> losAngeles = new NodeWrapper<string>("Los Angeles");
			NodeWrapper<string> canada = new NodeWrapper<string>("Canada");
			NodeWrapper<string> greenland = new NodeWrapper<string>("Greenland");
			NodeWrapper<string> southDakota = new NodeWrapper<string>("South Dakota");
			NodeWrapper<string> denmark = new NodeWrapper<string>("Denmark");

			//Americas nodes
			ohio.AddNeighbor(nevada, 2);
			ohio.AddNeighbor(losAngeles, 4);
			losAngeles.AddNeighbor(canada, 2);
			southDakota.AddNeighbor(losAngeles, 2);
			nevada.AddNeighbor(canada, 3);
			canada.AddNeighbor(greenland, 10);
			greenland.AddNeighbor(denmark, 6);

			PrintShortestRoute(ohio);
		}

		public static void PrintShortestRoute(NodeWrapper<string> startV) {
			Dictionary<NodeWrapper<string>, LinkedList<NodeWrapper<string>>> result = startV.ShortestRouteToOthers();
			foreach (KeyValuePair<NodeWrapper<string>, LinkedList<NodeWrapper<string>>> vertexDistance in result) {
				string routePrint = $"Shortest route to {vertexDistance.Key}: ";
				foreach (NodeWrapper<string> v in vertexDistance.Value) {
					routePrint += $"{v}, ";
				}
				Console.WriteLine(routePrint);
			}
		}
	}

	public class NodeWrapper<T> {
		public Dictionary<NodeWrapper<T>, int> Neighbors { get; }
		public T Data { get; set; }

		public NodeWrapper(T data) {
			Neighbors = new Dictionary<NodeWrapper<T>, int>();
			Data = data;
		}

		public void AddNeighbor(NodeWrapper<T> v, int distance) {
			Neighbors[v] = distance;
			v.Neighbors[this] = distance;
		}

		public bool RemoveNeighbor(NodeWrapper<T> v) {
			if (Neighbors.ContainsKey(v)) {
				Neighbors.Remove(v);
				v.Neighbors.Remove(this);

				return true;
			}
			return false;
		}

		public override string ToString() {
			return Data.ToString();
		}

		public Dictionary<NodeWrapper<T>, LinkedList<NodeWrapper<T>>> ShortestRouteToOthers() {
			//The dictionary that when finished will contain the shortest route to all of the Vs in the network
			Dictionary<NodeWrapper<T>, LinkedList<NodeWrapper<T>>> shortestRoutes = new Dictionary<NodeWrapper<T>, LinkedList<NodeWrapper<T>>>();
			//Dictionary that keeps track of Vs that have been the current
			Dictionary<NodeWrapper<T>, bool> finishedVertex = new Dictionary<NodeWrapper<T>, bool>();
			//Sorted list that keeps track of Vs with smallest distance from the starting V
			SortedList<int, NodeWrapper<T>> next = new SortedList<int, NodeWrapper<T>>(new DuplicateKeyComparer<int>());

			NodeWrapper<T> currentV = this;
			LinkedList<NodeWrapper<T>> currentRoute = new LinkedList<NodeWrapper<T>>();
			currentRoute.AddLast(currentV);

			//O(n)
			while (!finishedVertex.ContainsKey(currentV)) {
				//O(n)/O(1) depending on length of Neighbors
				foreach (NodeWrapper<T> v in currentV.Neighbors.Keys) {
					if (!finishedVertex.ContainsKey(v)) {
						int distanceToStartV = sumOfRoute(currentRoute) + v.Neighbors[currentV];

						LinkedList<NodeWrapper<T>> route = new LinkedList<NodeWrapper<T>>(currentRoute);
						route.AddLast(v);

						
						if (!shortestRoutes.ContainsKey(v)) {
							//O(log(n))
							next.Add(distanceToStartV, v);

							//O(1)
							shortestRoutes[v] = route;
						}
						
						else if (distanceToStartV < sumOfRoute(shortestRoutes[v])) {
							//O(1)
							shortestRoutes[v] = route;

							//O(n)
							//Could be shortened to O(log(n)) by accessing next by key (by using sumOfRouteMethod)
							int placementInNext = next.IndexOfValue(v);
							next.RemoveAt(placementInNext);

							//O(log(n))
							next.Add(distanceToStartV, v);
						}
					}
				}
				//current will always be the Vertex with smallest distance to startV and therefore finished
				finishedVertex[currentV] = true;
				//update distanceFromStartV to distance of next Vertex
				if (next.Count > 0) {
					currentV = next.First().Value;
					currentRoute = shortestRoutes[currentV];

					//Remove the new current from next list
					next.RemoveAt(0);
				}
			}


			return shortestRoutes;
		}

		private int sumOfRoute(LinkedList<NodeWrapper<T>> route) {
			int sum = 0;
			NodeWrapper<T> prev = null;

			foreach (NodeWrapper<T> v in route) {
				if (prev != null) {
					sum += v.Neighbors[prev];
				}
				prev = v;
			}

			return sum;
		}
	}

	public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable {
		public int Compare(TKey x, TKey y) {
			int result = x.CompareTo(y);

			if (result == 0)
				return 1;   // Handle equality as beeing greater
			else
				return result;
		}
	}
}
