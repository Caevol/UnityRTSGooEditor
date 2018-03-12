using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Pathfinder
    {

        const int MAX_STEPS = 700;

        private class Position : IComparable
        {
            public Position prevPosition;
            public Vector2 curPosition;
            public float stepsTaken;
            public float stepsFromGoal;

            public Position(Position prev, Vector2 cur, float stepsTotal, float stepsFrom)
            {
                prevPosition = prev;
                curPosition = cur;
                stepsTaken = stepsTotal;
                stepsFromGoal = stepsFrom;
            }

            public int CompareTo(object obj)
            {
                if (obj.GetType() != typeof(Position)) return 0;
                Position b = (Position)obj;
                if (stepsFromGoal - stepsTaken < b.stepsFromGoal - b.stepsTaken) return 1;
                else if (stepsFromGoal - stepsTaken > b.stepsFromGoal - b.stepsTaken) return -1;
                return 0;
            }
        }

        static public List<Vector2> getPath(Vector2 startpoint, Vector2 endpoint, BoardRunner board)
        {
            C5.IntervalHeap<Position> heap = new C5.IntervalHeap<Position>();
            //A* search, start with startpoint, make priorityqueue of positions, and keep adding
            Dictionary<int, Position> visitedPoints = new Dictionary<int, Position>(board.sizeX * board.sizeY);

            Position start = new Position(null, startpoint, 0, Vector2.Distance(startpoint, endpoint));
            heap.Add(start);
            visitedPoints[getPosition(startpoint, board)] = start;

            while (!heap.IsEmpty)
            {
                Position cur = heap.DeleteMax();
                if (cur.curPosition == endpoint)
                {
                    List<Vector2> solution = getSolution(cur);
                    return solution;
                }

                if (cur.stepsTaken > MAX_STEPS) return null;

                //up, down, left, right, diagonals
                explorePosition(endpoint, cur, visitedPoints, heap, board, 1, 0);
                explorePosition(endpoint, cur, visitedPoints, heap, board, -1, 0);
                explorePosition(endpoint, cur, visitedPoints, heap, board, 0, 1);
                explorePosition(endpoint, cur, visitedPoints, heap, board, 0, -1);
                explorePosition(endpoint, cur, visitedPoints, heap, board, 1, 1);
                explorePosition(endpoint, cur, visitedPoints, heap, board, 1, -1);
                explorePosition(endpoint, cur, visitedPoints, heap, board, -1, 1);
                explorePosition(endpoint, cur, visitedPoints, heap, board, -1, -1);
                
            }
            
            return null;
        }

        static void explorePosition(Vector2 endpoint, Position cur, Dictionary<int, Position> visitedPoints, C5.IntervalHeap<Position> heap, BoardRunner board, int offx, int offy)
        {
            Vector2 right = new Vector2(cur.curPosition.x + offx, cur.curPosition.y + offy);
            BoardTile g = board.getBoardTile(Mathf.RoundToInt(right.x), Mathf.RoundToInt(right.y));
            if (g != null && g.isTraversable(cur.curPosition))
            {
                BoardTile b = g.GetComponent<BoardTile>();

                if (!visitedPoints.ContainsKey(getPosition(new Vector2(cur.curPosition.x + offx, cur.curPosition.y + offy), board)))
                {
                    Position p = new Position(cur, right, cur.stepsTaken + Mathf.Sqrt(offx * offx + offy * offy), Vector2.Distance(right, endpoint));
                    visitedPoints[getPosition(right, board)] = p;
                    heap.Add(p);
                }
                else if (visitedPoints[getPosition(new Vector2(cur.curPosition.x + offx, cur.curPosition.y + offy), board)].stepsFromGoal
                            > cur.stepsFromGoal + Mathf.Sqrt(offx * offx + offy * offy)) //if this position can reach the tile in fewer steps, update the step count, and the prevPosition for that tile.
                {
                    visitedPoints[getPosition(new Vector2(cur.curPosition.x + offx, cur.curPosition.y + offy), board)].stepsFromGoal = cur.stepsFromGoal + Mathf.Sqrt(offx * offx + offy * offy);
                    visitedPoints[getPosition(new Vector2(cur.curPosition.x + offx, cur.curPosition.y + offy), board)].prevPosition = cur;
                }

            }
        }

        static List<Vector2> getSolution(Position pos)
        {
            List<Vector2> result = new List<Vector2>();
            Position ptr = pos;
            while(ptr != null)
            {
                result.Insert(0, ptr.curPosition);
                ptr = ptr.prevPosition;
            }

            return result;

            /*if (pos.prevPosition == null) return new List<Vector2>(1) { pos.curPosition };

            else
            {
                List<Vector2> lst = getSolution(pos.prevPosition);
                lst.Add(pos.curPosition);
                return lst;
            }
            */
        }

        static int getPosition(Vector2 pos, BoardRunner b)
        {
            return Mathf.RoundToInt(pos.x) + Mathf.RoundToInt(pos.y) * b.sizeY;
        }



    }
}
