using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    // 모든 노드의 최상위 인터페이스
    public interface INode
    {
        bool Run();
    }

    // 수행 노드
    public interface ITeskNode : INode { }

    // 조건 노드
    public interface IConditionNode : INode { }

    // 전이 노드
    public interface ICompositeNode : INode { }
}