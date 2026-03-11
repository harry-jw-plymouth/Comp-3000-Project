using UnityEngine;

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Road Rule Tile")]
public class RoadRuleTile : RuleTile
{
    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (neighbor == TilingRule.Neighbor.This)
        {
            return other is RoadRuleTile;
        }

        return base.RuleMatch(neighbor, other);
    }
}