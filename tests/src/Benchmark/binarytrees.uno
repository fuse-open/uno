/*
      The Computer Language Benchmarks Game
      http://benchmarksgame.alioth.debian.org/

      contributed by Marek Safar
      optimized by kasthack
*/
using Uno;
using Uno.IO;
using Uno.Testing;

public class BinaryTrees {
   const int minDepth = 4;

   [Test]
   public void Main() {
      int n = 12;
      int maxDepth = Math.Max(minDepth + 2, n);
      int stretchDepth = maxDepth + 1;
      int check = ( TreeNode.bottomUpTree(0, stretchDepth) ).itemCheck();
      //Console.WriteLine("stretch tree of depth {0}\t check: {1}", stretchDepth, check);
      TreeNode longLivedTree = TreeNode.bottomUpTree(0, maxDepth);
      for ( int depth = minDepth; depth <= maxDepth; depth += 2 ) {
         int iterations = 1 << ( maxDepth - depth + minDepth );
         check = 0;
         for ( int i = 1; i <= iterations; i++ ) {
            check += ( TreeNode.bottomUpTree(i, depth) ).itemCheck();
            check += ( TreeNode.bottomUpTree(-i, depth) ).itemCheck();
         }
         //Console.WriteLine("{0}\t trees of depth {1}\t check: {2}",
         //    iterations * 2, depth, check);
      }
      //Console.WriteLine("long lived tree of depth {0}\t check: {1}",
      //    maxDepth, longLivedTree.itemCheck());
   }

   class TreeNode {
      private TreeNode left, right;
      private int item;
      TreeNode( int item ) {
         this.item = item;
      }
      internal static TreeNode bottomUpTree( int item, int depth ) {
         TreeNode t;
         ChildTreeNodes(out t, item, depth - 1);
         return t;
      }
      static void ChildTreeNodes( out TreeNode node, int item, int depth ) {
         node = new TreeNode(item);
         if ( depth > 0 ) {
            ChildTreeNodes(out node.left, 2 * item - 1, depth - 1);
            ChildTreeNodes(out node.right, 2 * item, depth - 1);
         }
      }
      internal int itemCheck() {
         if ( right == null ) return item;
         else return item + left.itemCheck() - right.itemCheck();
      }
   }
}
