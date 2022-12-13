using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorTree
{
    // 수행 노드
    public class TeskNode : ITeskNode
    {
        public Action Tesk { get; set; }

        public TeskNode(Action _tesk)
        {
            Tesk = _tesk;
        }

        public virtual bool Run()
        {
            Tesk();
            return true;
        }
    }

    // 조건 노드
    public class ConditionNode : IConditionNode
    {
        public Func<bool> Condition { get; protected set; }

        public ConditionNode(Func<bool> _condition)
        {
            Condition = _condition;
        }

        public bool Run() => Condition();

        public ConditionalSingleTeskNode Tesk(Action _tesk)
            => new ConditionalSingleTeskNode(Condition, _tesk);

        public ConditionalSequenceNode Sequence(params INode[] _nodes)
            => new ConditionalSequenceNode(Condition, _nodes);

        public ConditionalSelectionNode Selection(params INode[] _nodes)
            => new ConditionalSelectionNode(Condition, _nodes);

        public ConditionalParallelNode Parallel(params INode[] _nodes)
            => new ConditionalParallelNode(Condition, _nodes);
    }

    // 조건부 단일 수행 노드 ( 조건이  true면 Tesk 수행 및 true 반환, false면 false 반환 ) 
    public class ConditionalSingleTeskNode : TeskNode
    {
        public Func<bool> Condition { get; private set; }

        public ConditionalSingleTeskNode(Func<bool> _condition, Action _action)
            : base(_action)
        {
            Condition = _condition;
        }

        public ConditionalSingleTeskNode(ConditionNode _conditionNode, TeskNode _teskNode)
            : base(_teskNode.Tesk)
        {
            Condition = _conditionNode.Condition;
        }

        public override bool Run()
        {
            bool result = Condition();
            if (result) Tesk();
            return result;
        }
    }

    // 조건부 선택 수행 노드 ( 조건이  true면 TrueTesk 수행 및 true 반환, false면 FalseTesk 수행 및 false 반환 )
    public class ConditionalDualTeskNode : ITeskNode
    {
        public Func<bool> Condition { get; private set; }
        public Action TrueTesk { get; private set; }
        public Action FalseTesk { get; private set; }

        public ConditionalDualTeskNode(Func<bool> _condition, Action _true, Action _false)
        {
            Condition = _condition;
            TrueTesk = _true;
            FalseTesk = _false;
        }

        public ConditionalDualTeskNode(ConditionNode _condition, TeskNode _true, TeskNode _false)
        {
            Condition = _condition.Condition;
            TrueTesk = _true.Tesk;
            FalseTesk = _false.Tesk;
        }

        public bool Run()
        {
            bool result = Condition();

            if (result) TrueTesk();
            else FalseTesk();

            return result;
        }
    }

    // 조건부 반전 수행 노드 ( 조건이 false면 Tesk 실행 및 true 반환, true면 false 반환 )
    public class ConditionalReverseTeskNode : ITeskNode
    {
        public Func<bool> Condition { get; private set; }
        public Action Tesk { get; private set; }

        public ConditionalReverseTeskNode(Func<bool> _condition, Action _tesk)
        {
            Condition = () => !_condition();
            Tesk = _tesk;
        }

        public ConditionalReverseTeskNode(ConditionNode _condition, TeskNode _tesk)
        {
            Condition = () => !_condition.Condition();
            Tesk = _tesk.Tesk;
        }

        public bool Run()
        {
            bool result = Condition();
            if (result) Tesk();
            return result;
        }
    }

    // 조건부 순차적 전이 노드
    public class ConditionalSequenceNode : DecoratedCompositeNode
    {
        public ConditionalSequenceNode(Func<bool> _condition, params INode[] _nodes)
            : base(_condition, new SequenceNode(_nodes)) { }
    }

    // 조건부 선택적 전이 노드
    public class ConditionalSelectionNode : DecoratedCompositeNode
    {
        public ConditionalSelectionNode(Func<bool> _condition, params INode[] _nodes)
            : base(_condition, new SelectionNode(_nodes)) { }
    }

    // 조건부 평행적 전이 노드
    public class ConditionalParallelNode : DecoratedCompositeNode
    {
        public ConditionalParallelNode(Func<bool> _condition, params INode[] _nodes)
            : base(_condition, new ParallelNode(_nodes)) { }
    }

    // 전이 노드 ( 전이 유형에 따라 자식 클래스 생성 필요 )
    public abstract class CompositeNode : ICompositeNode
    {
        public List<INode> Childs { get; protected set; }

        public CompositeNode(params INode[] _nodes) => Childs = new List<INode>(_nodes);

        public CompositeNode AddNode(INode _node)
        {
            Childs.Add(_node);
            return this;
        }
        public abstract bool Run();
    }

    // 조건부 전이 노드
    public abstract class DecoratedCompositeNode : CompositeNode
    {
        public Func<bool> Condition { get; protected set; }

        public CompositeNode Composite { get; protected set; }

        public DecoratedCompositeNode(Func<bool> _condition, CompositeNode _composite)
        {
            Condition = _condition;
            Composite = _composite;
        }

        public override bool Run()
        {
            if(!Condition())
            {
                return false;
            }
            return Composite.Run();
        }
    }

    // 평행적 순회 노드 ( 자식의 반환 값과 관계 없이 순회 )
    public class ParallelNode : CompositeNode
    {
        public ParallelNode(params INode[] _nodes) 
            : base(_nodes) { }

        public override bool Run()
        {
            foreach(INode node in Childs)
            {
                node.Run();
            }
            return true;
        }
    }

    // 선택적 순회 노드 ( 자식 중 반환 값이 true인 경우만 수행 )
    public class SelectionNode : CompositeNode
    {
        public SelectionNode(params INode[] _nodes)
            : base(_nodes) { }

        public override bool Run()
        {
            foreach (INode node in Childs)
            {
                bool result = node.Run();
                if(result)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // 순차적 순회 노드 ( 자식 중 반환 값이 false가 나올 때 까지 순회 )
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(params INode[] _nodes) 
            : base(_nodes) { }

        public override bool Run()
        {
            foreach (INode node in Childs)
            {
                bool result = node.Run();
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
    }

    // 비헤이비어 트리 노드를 생성하는 스태틱 클래스
    public static class NodeCreator
    {
        public static SelectionNode Selection(params INode[] _nodes) => new SelectionNode(_nodes);
        public static SequenceNode Sequence(params INode[] _nodes) => new SequenceNode(_nodes);
        public static ParallelNode Parallel(params INode[] _nodes) => new ParallelNode(_nodes);

        public static ConditionNode Condition(Func<bool> _condition) => new ConditionNode(_condition);
        public static TeskNode Tesk(Action _tesk) => new TeskNode(_tesk);

        public static ConditionalSingleTeskNode SingleConTesk(Func<bool> _condition, Action _tesk)
            => new ConditionalSingleTeskNode(_condition, _tesk);
        public static ConditionalDualTeskNode DualConTesk(Func<bool> _condition, Action _true, Action _false)
            => new ConditionalDualTeskNode(_condition, _true, _false);
        public static ConditionalReverseTeskNode ReverseConTesk(Func<bool> _condition, Action _tesk)
            => new ConditionalReverseTeskNode(_condition, _tesk);
    }

}
