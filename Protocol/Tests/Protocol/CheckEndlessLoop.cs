namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckEndlessLoop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckEndlessLoop, Category.Protocol)]
    internal class CheckEndlessLoop : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            var model = context.ProtocolModel;

            var triggers = context.EachTriggerWithValidId().ToList();
            LoopInfo info = new LoopInfo(this, model, context, triggers, results);

            foreach (ITriggersTrigger trigger in triggers)
            {
                info.Clean();
                info.CreatePath(ItemTypes.Trigger, trigger.Id.Value.Value, info.Paths[0]);
            }

            var actions = context.EachActionWithValidId().ToList();
            foreach (IActionsAction action in actions)
            {
                info.Clean();
                info.CreatePath(ItemTypes.Action, action.Id.Value.Value, info.Paths[0]);
            }

            return results;
        }

        private sealed class LoopInfo
        {
            #region Fields

            private readonly ValidatorContext _validatorContext;
            private readonly IProtocol _protocol;
            private readonly List<IValidationResult> _results;
            private readonly CheckEndlessLoop _ref;
            private readonly List<ITriggersTrigger> _triggers;

            #endregion Fields

            #region Constructor

            public LoopInfo(CheckEndlessLoop checkEndlessLoop, IProtocolModel model, ValidatorContext validatorContext, List<ITriggersTrigger> triggers, List<IValidationResult> results)
            {
                _validatorContext = validatorContext;
                _protocol = model.Protocol;
                _results = results;
                _ref = checkEndlessLoop;
                _triggers = triggers;
                PathClass start = new PathClass();
                Paths = new List<PathClass>();
                HandledItems = new List<IReadable>();
                Paths.Add(start);
            }

            #endregion Constructor

            #region Properties

            public List<IReadable> HandledItems { get; }

            public List<PathClass> Paths { get; private set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// The loop to go through all flows.
            /// </summary>
            /// <param name="currentItemType">Current item type.</param>
            /// <param name="currentItemId">Current item ID.</param>
            /// <param name="pathClass">Path Class.</param>
            public void CreatePath(ItemTypes currentItemType, uint currentItemId, PathClass pathClass)
            {
                var currentItem = GetItem(currentItemType, currentItemId);
                if (currentItem == null)
                {
                    return;
                }

                if (IsItemHandled(currentItem))
                {
                    return;
                }

                bool hasPassedAlready = pathClass.ContainsItemInPath(currentItemType, currentItemId);
                if (hasPassedAlready)
                {
                    pathClass.Path.Add(new Info(currentItemId, currentItemType));
                    if (pathClass.HasCondition)
                    {
                        _results.Add(Error.PotentialEndlessLoop(_ref, currentItem, currentItem, pathClass.PathToString()));
                    }
                    else
                    {
                        _results.Add(Error.EndlessLoop(_ref, currentItem, currentItem, pathClass.PathToString()));
                    }

                    return;
                }

                switch (currentItemType)
                {
                    case ItemTypes.Trigger:
                        TriggerFlow(pathClass, currentItem as ITriggersTrigger);
                        break;

                    case ItemTypes.Action:
                        ActionFlow(pathClass, currentItem as IActionsAction);
                        break;

                    case ItemTypes.Group:
                        GroupFlow(currentItemId, pathClass, currentItem as IGroupsGroup);
                        break;

                    case ItemTypes.QAction:
                        QActionFlow(currentItemId, pathClass);
                        break;

                    case ItemTypes.Command:
                        CommandFlow(currentItemId, pathClass, currentItem as ICommandsCommand);
                        break;

                    case ItemTypes.Response:
                        ResponseFlow(currentItemId, pathClass, currentItem as IResponsesResponse);
                        break;

                    case ItemTypes.Pair:
                        PairFlow(currentItemId, pathClass, currentItem as IPairsPair);
                        break;

                    case ItemTypes.Param:
                        ParameterFlow(currentItemId, pathClass, currentItem as IParamsParam);
                        break;

                    case ItemTypes.Timer:
                        TimerFlow(currentItemId, pathClass, currentItem as ITimersTimer);
                        break;

                    case ItemTypes.Session:
                        SessionFlow(currentItemId, pathClass, currentItem as IHTTPSession);
                        break;

                    default: break;
                }

                HandledItems.Add(currentItem);
            }

            /// <summary>
            /// The flow when the check comes to an action.
            /// </summary>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="action">The action.</param>
            private void ActionFlow(PathClass pathClass, IActionsAction action)
            {
                ItemTypes nextType = ItemTypes.Action;

                if (action?.On?.Value == null)
                {
                    return;
                }

                if (action.Type?.Value == null)
                {
                    return;
                }

                EnumActionOn on = action.On.Value.Value;
                var onIds = action.On.GetId();

                EnumActionType type = action.Type.Value.Value;

                string condition = action.Condition?.Value;
                if (!String.IsNullOrWhiteSpace(condition))
                {
                    pathClass.HasCondition = true;
                }

                // The "On" value type
                switch (on)
                {
                    case EnumActionOn.Parameter:
                        EnumActionType[] allowedTypes = new[]
                        {
                            EnumActionType.Aggregate, // should be added, but needs to wait onto a change by MOD
                            EnumActionType.ChangeLength,
                            EnumActionType.Go,
                            EnumActionType.Increment,
                            EnumActionType.Multiply,
                            EnumActionType.Pow,
                            EnumActionType.RunActions
                        };

                        if (allowedTypes.Contains(type))
                        {
                            nextType = ItemTypes.Param;
                            if (type == EnumActionType.RunActions)
                            {
                                pathClass.IsRunAction = true;
                            }
                        }

                        break;

                    case EnumActionOn.Pair:
                        EnumActionType[] excludedPairTypes =
                        {
                            EnumActionType.Timeout,
                            EnumActionType.SetNext
                        };

                        if (!excludedPairTypes.Contains(type))
                        {
                            // Continue path if the type is not excluded.
                            nextType = ItemTypes.Pair;
                        }

                        break;

                    case EnumActionOn.Command:
                        if (action.Type?.Value != EnumActionType.Make && action.Type?.Value != EnumActionType.Crc)
                        {
                            nextType = ItemTypes.Command;
                        }

                        break;

                    case EnumActionOn.Group:
                        nextType = ItemTypes.Group;
                        if (action.Type?.Value == EnumActionType.Set)
                        {
                            pathClass.IsActionSet = true;
                        }

                        break;

                    case EnumActionOn.Response:
                        break;

                    case EnumActionOn.Timer:
                        if (type != EnumActionType.Stop)
                        {
                            nextType = ItemTypes.Timer;
                        }

                        break;

                    default: break;
                }

                if (action.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(action.Id.Value.Value, ItemTypes.Action));
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                if (nextType == ItemTypes.Action || onIds.Count == 0)
                {
                    return;
                }

                // All "on" links
                if (nextType == ItemTypes.Param && type == EnumActionType.Aggregate)
                {
                    if (action.Type.Options == null)
                    {
                        return;
                    }

                    var typeOption = action.Type?.Options.Value;
                    if (UInt32.TryParse(typeOption, out uint param))
                    {
                        CreatePath(nextType, param, startPath);
                    }
                    else
                    {
                        var aggregateOptions = action.Type?.GetOptionsByType()?.Aggregate;

                        if (aggregateOptions?.DefaultIf?.Value != null)
                        {
                            pathClass.HasCondition = true;
                        }

                        var defaultValue = aggregateOptions?.DefaultValue?.ColumnPid;
                        var returnValues = aggregateOptions?.Return;
                        var groupbyValues = aggregateOptions?.GroupBy?.Values;
                        var ids = GetAggregationTriggers(defaultValue, returnValues, groupbyValues);

                        for (int i = 0; i < ids.Length; i++)
                        {
                            var currentPath = pathClass;
                            if (i != 0)
                            {
                                currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                                Paths.Add(currentPath);
                            }

                            CreatePath(nextType, ids[i], currentPath);
                        }
                    }

                    //After change by MOD this should be updated to continue the flow based on the option.
                }
                else
                {
                    for (int i = 0; i < onIds.Count; i++)
                    {
                        var currentPath = pathClass;
                        if (i != 0)
                        {
                            currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(currentPath);
                        }

                        CreatePath(nextType, onIds[i], currentPath);
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a trigger.
            /// </summary>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="trigger">The trigger.</param>
            private void TriggerFlow(PathClass pathClass, ITriggersTrigger trigger)
            {
                // Add new trigger to the flow
                if (trigger.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(trigger.Id.Value.Value, ItemTypes.Trigger));
                }
                else
                {
                    return;
                }

                string condition = trigger.Condition?.Value;
                if (!String.IsNullOrWhiteSpace(condition))
                {
                    pathClass.HasCondition = true;
                }

                var content = trigger.Content;
                if (content == null)
                {
                    return;
                }

                ItemTypes triggerType;
                if (trigger.Type?.Value != null)
                {
                    triggerType = trigger.Type.Value == EnumTriggerType.Action ? ItemTypes.Action : ItemTypes.Trigger;
                }
                else
                {
                    triggerType = ItemTypes.Action;
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);

                // Loop through the content of the Trigger
                for (int i = 0; i < content.Count; i++)
                {
                    if (!content[i].Value.HasValue)
                    {
                        continue;
                    }

                    if (i != 0)
                    {
                        var newPath = new PathClass(startPath.Path, startPath.HasCondition);
                        Paths.Add(newPath);
                        CreatePath(triggerType, content[i].Value.Value, newPath);
                    }
                    else
                    {
                        CreatePath(triggerType, content[i].Value.Value, pathClass);
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a group.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="group">The group.</param>
            private void GroupFlow(uint id, PathClass pathClass, IGroupsGroup group)
            {
                int counter = 0;

                FindTriggersOnId(EnumTriggerOn.Group, id, out List<ITriggersTrigger> triggersBefore, out List<ITriggersTrigger> triggersAfter);

                string condition = group.Condition?.Value;
                if (!String.IsNullOrWhiteSpace(condition))
                {
                    pathClass.HasCondition = true;
                }

                if (group.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(group.Id.Value.Value, ItemTypes.Group));
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);

                if (triggersBefore.Count > 0)
                {
                    // Flow with before trigger, content, after trigger.
                    for (int i = 0; i < triggersBefore.Count; i++)
                    {
                        var beforePath = pathClass;
                        if (i != 0)
                        {
                            beforePath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(beforePath);
                        }

                        if (triggersBefore[i].Id != null && triggersBefore[i].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersBefore[i].Id.Value.Value, beforePath);
                        }

                        var startPathContent = new PathClass(beforePath.Path, beforePath.HasCondition);

                        if (group.Content == null || pathClass.IsActionSet)
                        {
                            pathClass.IsActionSet = false;
                            continue;
                        }

                        for (int k = 0; k < group.Content.Count; k++)
                        {
                            var currentPath = beforePath;
                            if (counter != 0)
                            {
                                currentPath = new PathClass(startPathContent.Path, startPathContent.HasCondition);
                                Paths.Add(currentPath);
                            }

                            var contentItem = group.Content[k];
                            uint contentId;
                            switch (contentItem)
                            {
                                case IGroupsGroupContentAction contentAction:
                                    if (UInt32.TryParse(contentAction.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Action, contentId, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentPair contentPair:
                                    if (contentPair.Value.HasValue)
                                    {
                                        CreatePath(ItemTypes.Pair, contentPair.Value.Value, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentParam contentParam:
                                    if (UInt32.TryParse(contentParam.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Param, contentId, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentSession contentSession:
                                    if (contentSession.Value.HasValue)
                                    {
                                        CreatePath(ItemTypes.Session, contentSession.Value.Value, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentTrigger contentTrigger:
                                    if (UInt32.TryParse(contentTrigger.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Trigger, contentId, currentPath);
                                    }

                                    break;

                                default: break;
                            }

                            var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                            for (int j = 0; j < triggersAfter.Count; j++)
                            {
                                var afterpath = currentPath;
                                if (j != 0)
                                {
                                    afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                    Paths.Add(afterpath);
                                }

                                if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                                {
                                    CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                                }
                            }

                            counter++;
                        }
                    }
                }
                else if ((group.Content == null || group.Content.Count == 0) && triggersAfter.Count > 0)
                {
                    // The flow when only after Trigger is present.
                    pathClass.IsActionSet = false;

                    var startPathAfter = new PathClass(pathClass.Path, pathClass.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = pathClass;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }
                }
                else
                {
                    // Path if no before or after trigger is present.
                    if (group.Content == null || pathClass.IsActionSet)
                    {
                        pathClass.IsActionSet = false;
                    }
                    else
                    {
                        for (int k = 0; k < group.Content.Count; k++)
                        {
                            var currentPath = pathClass;
                            if (counter != 0)
                            {
                                currentPath = new PathClass(startPath.Path, pathClass.HasCondition);

                                Paths.Add(currentPath);
                            }

                            var contentItem = group.Content[k];
                            uint contentId;
                            switch (contentItem)
                            {
                                case IGroupsGroupContentAction contentAction:
                                    if (UInt32.TryParse(contentAction.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Action, contentId, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentPair contentPair:
                                    if (contentPair.Value.HasValue)
                                    {
                                        CreatePath(ItemTypes.Pair, contentPair.Value.Value, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentParam contentParam:
                                    if (UInt32.TryParse(contentParam.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Param, contentId, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentSession contentSession:
                                    if (contentSession.Value.HasValue)
                                    {
                                        CreatePath(ItemTypes.Session, contentSession.Value.Value, currentPath);
                                    }

                                    break;

                                case IGroupsGroupContentTrigger contentTrigger:
                                    if (UInt32.TryParse(contentTrigger.Value, out contentId))
                                    {
                                        CreatePath(ItemTypes.Trigger, contentId, currentPath);
                                    }

                                    break;

                                default: break;
                            }

                            var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                            for (int j = 0; j < triggersAfter.Count; j++)
                            {
                                var afterpath = currentPath;
                                if (j != 0)
                                {
                                    afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                    Paths.Add(afterpath);
                                }

                                if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                                {
                                    CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                                }
                            }

                            counter++;
                        }
                    }

                    if (triggersAfter != null && triggersAfter.Count > 0)
                    {
                        var startPathAfter = new PathClass(pathClass.Path, pathClass.HasCondition);
                        for (int j = 0; j < triggersAfter.Count; j++)
                        {
                            var afterpath = pathClass;
                            if (j != 0)
                            {
                                afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                Paths.Add(afterpath);
                            }

                            if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                            {
                                CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a command.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="command">The command.</param>
            private void CommandFlow(uint id, PathClass pathClass, ICommandsCommand command)
            {
                List<ITriggersTrigger> triggersBefore;
                List<ITriggersTrigger> triggersAfter;
                FindTriggersOnId(EnumTriggerOn.Command, id, out triggersBefore, out triggersAfter);
                if (command.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(command.Id.Value.Value, ItemTypes.Command));
                }

                if (triggersBefore.Count > 0)
                {
                    // The before flow
                    var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                    for (int i = 0; i < triggersBefore.Count; i++)
                    {
                        var beforePath = pathClass;
                        if (i != 0)
                        {
                            beforePath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(beforePath);
                        }

                        if (triggersBefore[i].Id != null && triggersBefore[i].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersBefore[i].Id.Value.Value, beforePath);
                        }

                        var startPathAfter = new PathClass(beforePath.Path, beforePath.HasCondition);

                        // The after trigger content.
                        for (int j = 0; j < triggersAfter.Count; j++)
                        {
                            var afterpath = beforePath;
                            if (j != 0)
                            {
                                afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                Paths.Add(afterpath);
                            }

                            if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                            {
                                CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                            }
                        }
                    }
                }
                else
                {
                    // When no before trigger was available.

                    var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = pathClass;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a response.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="response">The response.</param>
            private void ResponseFlow(uint id, PathClass pathClass, IResponsesResponse response)
            {
                int counter = 0;

                List<ITriggersTrigger> triggersBefore;
                List<ITriggersTrigger> triggersAfter;
                FindTriggersOnId(EnumTriggerOn.Response, id, out triggersBefore, out triggersAfter);

                if (response.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(response.Id.Value.Value, ItemTypes.Response));
                }

                if (triggersBefore.Count > 0)
                {
                    var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                    for (int i = 0; i < triggersBefore.Count; i++)
                    {
                        // The before trigger flow
                        var beforePath = pathClass;
                        if (i != 0)
                        {
                            beforePath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(beforePath);
                        }

                        if (triggersBefore[i].Id != null && triggersBefore[i].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersBefore[i].Id.Value.Value, beforePath);
                        }

                        var startPathContent = new PathClass(beforePath.Path, beforePath.HasCondition);
                        foreach (var contentItem in response.Content)
                        {
                            var currentPath = beforePath;
                            if (counter != 0)
                            {
                                currentPath = new PathClass(startPathContent.Path, startPathContent.HasCondition);
                                Paths.Add(currentPath);
                            }

                            if (contentItem.Value == null)
                            {
                                continue;
                            }

                            if (contentItem.Value.HasValue)
                            {
                                CreatePath(ItemTypes.Param, contentItem.Value.Value, currentPath);
                            }

                            var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                            for (int j = 0; j < triggersAfter.Count; j++)
                            {
                                var afterpath = currentPath;
                                if (j != 0)
                                {
                                    afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                    Paths.Add(afterpath);
                                }

                                if (triggersAfter[j].Id?.Value != null)
                                {
                                    CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                                }
                            }

                            counter++;
                        }
                    }
                }
                else if (response.Content == null || response.Content.Count == 0)
                {
                    // When the content is empty or not existing.
                    var startPathAfter = new PathClass(pathClass.Path, pathClass.HasCondition);

                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = pathClass;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id?.Value != null)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }
                }
                else
                {
                    // Containing content flow -> default flow
                    var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                    foreach (var contentItem in response.Content)
                    {
                        var currentPath = pathClass;
                        if (counter != 0)
                        {
                            currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(currentPath);
                        }

                        if (contentItem.Value == null)
                        {
                            continue;
                        }

                        if (contentItem.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Param, contentItem.Value.Value, currentPath);
                        }

                        var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                        for (int j = 0; j < triggersAfter.Count; j++)
                        {
                            var afterpath = currentPath;
                            if (j != 0)
                            {
                                afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                                Paths.Add(afterpath);
                            }

                            if (triggersAfter[j].Id?.Value != null)
                            {
                                CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                            }
                        }

                        counter++;
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a pair.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="pair">The pair.</param>
            private void PairFlow(uint id, PathClass pathClass, IPairsPair pair)
            {
                var triggersAfter = _triggers.Where(x =>
                     x.On?.Value == EnumTriggerOn.Pair && IsTriggeredOnEachOrId(x, id)
                     && (EnumTriggerTimeConverter.Convert(x.Time?.Value) == EnumTriggerTime.Succeeded
                     || EnumTriggerTimeConverter.Convert(x.Time?.Value) == EnumTriggerTime.Timeout
                     || EnumTriggerTimeConverter.Convert(x.Time?.Value) == EnumTriggerTime.TimeoutAfterRetries)
                 ).ToList();

                if (pair.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(pair.Id.Value.Value, ItemTypes.Pair));
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);

                // Check if content is empty.
                if (pair.Content == null || pair.Content.Count == 0)
                {
                    var currentPath = pathClass;
                    var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = currentPath;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }

                    return;
                }

                int counter = 0;
                foreach (var contentItem in pair.Content)
                {
                    var currentPath = pathClass;

                    if (counter != 0)
                    {
                        currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                        Paths.Add(currentPath);
                    }

                    switch (contentItem)
                    {
                        case IPairsPairContentCommand contentCommand:
                            if (contentCommand.Value.HasValue)
                            {
                                CreatePath(ItemTypes.Command, contentCommand.Value.Value, currentPath);
                            }

                            break;

                        case IPairsPairContentResponse contentResponse:
                            if (contentResponse.Value.HasValue)
                            {
                                CreatePath(ItemTypes.Response, contentResponse.Value.Value, currentPath);
                            }

                            break;

                        default: break;
                    }

                    var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = currentPath;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }

                    counter++;
                }
            }

            /// <summary>
            /// The flow when the check comes to a Timer.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="timer">The timer.</param>
            private void TimerFlow(uint id, PathClass pathClass, ITimersTimer timer)
            {
                int counter = 0;

                var triggersBefore = _triggers.Where(t =>
                    t.On?.Value == EnumTriggerOn.Timer && IsTriggeredOnEachOrId(t, id)
                    && EnumTriggerTimeConverter.Convert(t.Time?.Value) == EnumTriggerTime.Before
                    ).ToList();

                if (timer.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(timer.Id.Value.Value, ItemTypes.Timer));
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);
                if (triggersBefore.Count > 0)
                {
                    for (int i = 0; i < triggersBefore.Count; i++)
                    {
                        var beforePath = pathClass;
                        if (i != 0)
                        {
                            beforePath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(beforePath);
                        }

                        if (triggersBefore[i].Id != null && triggersBefore[i].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersBefore[i].Id.Value.Value, beforePath);
                        }

                        var startPathContent = new PathClass(beforePath.Path, beforePath.HasCondition);
                        foreach (var contentItem in timer.Content)
                        {
                            var currentPath = beforePath;
                            if (counter != 0)
                            {
                                currentPath = new PathClass(startPathContent.Path, startPathContent.HasCondition);
                                Paths.Add(currentPath);
                            }

                            if (UInt32.TryParse(contentItem.Value, out uint contentId))
                            {
                                CreatePath(ItemTypes.Group, contentId, currentPath);
                            }

                            counter++;
                        }
                    }
                }
                else
                {
                    foreach (var contentItem in timer.Content)
                    {
                        var currentPath = pathClass;
                        if (counter != 0)
                        {
                            currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(currentPath);
                        }

                        if (UInt32.TryParse(contentItem.Value, out uint contentId))
                        {
                            CreatePath(ItemTypes.Group, contentId, currentPath);
                        }

                        counter++;
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a parameter.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="parameter">The parameter.</param>
            private void ParameterFlow(uint id, PathClass pathClass, IParamsParam parameter)
            {
                List<EnumTriggerTime?> allowedTriggerTimes = new List<EnumTriggerTime?>
                {
                    EnumTriggerTime.Change,
                    EnumTriggerTime.ChangeAfterResponse,
                    EnumTriggerTime.Timeout,
                    EnumTriggerTime.TimeoutAfterRetries,
                };

                var triggersAfter = _triggers.Where(t =>
                    t.On?.Value == EnumTriggerOn.Parameter && IsTriggeredOnEachOrId(t, id)
                    && t.Time?.Value != null && allowedTriggerTimes.Contains(EnumTriggerTimeConverter.Convert(t.Time.Value))).ToList();

                var qactionTriggers = _validatorContext.EachQActionWithValidId().Where(x => x.GetTriggers().Contains(id)).ToList();

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);

                if (parameter.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(parameter.Id.Value.Value, ItemTypes.Param));
                }

                if (pathClass.IsRunAction)
                {
                    triggersAfter.Clear();
                    pathClass.IsRunAction = false;
                }
                else
                {
                    pathClass.IsRunAction = false;
                }

                if (triggersAfter.Count > 0)
                {
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = pathClass;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }

                        var startPathQaction = new PathClass(afterpath.Path, afterpath.HasCondition);
                        for (int k = 0; k < qactionTriggers.Count; k++)
                        {
                            var qactionPath = afterpath;
                            if (k != 0)
                            {
                                qactionPath = new PathClass(startPathQaction.Path, startPathQaction.HasCondition);
                                Paths.Add(qactionPath);
                            }

                            if (qactionTriggers[k].Id != null && qactionTriggers[k].Id.Value.HasValue)
                            {
                                CreatePath(ItemTypes.QAction, qactionTriggers[k].Id.Value.Value, qactionPath);
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < qactionTriggers.Count; k++)
                    {
                        var qactionPath = pathClass;
                        if (k != 0)
                        {
                            qactionPath = new PathClass(startPath.Path, startPath.HasCondition);
                            Paths.Add(qactionPath);
                        }

                        if (qactionTriggers[k].Id != null && qactionTriggers[k].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.QAction, qactionTriggers[k].Id.Value.Value, qactionPath);
                        }
                    }
                }
            }

            /// <summary>
            /// The flow when the check comes to a parameter.
            /// </summary>
            /// <param name="id">QAction Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            private void QActionFlow(uint id, PathClass pathClass)
            {
                pathClass.Path.Add(new Info(id, ItemTypes.QAction));
            }

            /// <summary>
            /// The flow when the check comes to a session.
            /// </summary>
            /// <param name="id">Trigger id.</param>
            /// <param name="pathClass">TriggerPath object of current path.</param>
            /// <param name="session">The session.</param>
            private void SessionFlow(uint id, PathClass pathClass, IHTTPSession session)
            {
                var triggersAfter = _triggers.Where(t =>
                    t.On?.Value == EnumTriggerOn.Session && IsTriggeredOnEachOrId(t, id)
                    && EnumTriggerTimeConverter.Convert(t.Time?.Value) == EnumTriggerTime.Timeout
                    ).ToList();

                if (session.Id?.Value != null)
                {
                    pathClass.Path.Add(new Info(session.Id.Value.Value, ItemTypes.Session));
                }

                var startPath = new PathClass(pathClass.Path, pathClass.HasCondition);

                int counter = 0;
                foreach (var contentItem in session)
                {
                    var currentPath = pathClass;
                    if (counter != 0)
                    {
                        currentPath = new PathClass(startPath.Path, startPath.HasCondition);
                        Paths.Add(currentPath);
                    }

                    if (contentItem?.Response?.Content?.Pid?.Value != null)
                    {
                        CreatePath(ItemTypes.Param, contentItem.Response.Content.Pid.Value.Value, currentPath);
                    }

                    if (contentItem?.Response?.StatusCode?.Value != null)
                    {
                        CreatePath(ItemTypes.Param, contentItem.Response.StatusCode.Value.Value, currentPath);
                    }

                    var startPathAfter = new PathClass(startPath.Path, startPath.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = startPathAfter;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }

                    counter++;
                }

                if (session.Count == 0)
                {
                    var currentPath = pathClass;
                    var startPathAfter = new PathClass(currentPath.Path, currentPath.HasCondition);
                    for (int j = 0; j < triggersAfter.Count; j++)
                    {
                        var afterpath = currentPath;
                        if (j != 0)
                        {
                            afterpath = new PathClass(startPathAfter.Path, startPathAfter.HasCondition);
                            Paths.Add(afterpath);
                        }

                        if (triggersAfter[j].Id != null && triggersAfter[j].Id.Value.HasValue)
                        {
                            CreatePath(ItemTypes.Trigger, triggersAfter[j].Id.Value.Value, afterpath);
                        }
                    }
                }
            }

            private void FindTriggersOnId(EnumTriggerOn onType, uint onId, out List<ITriggersTrigger> triggersBefore, out List<ITriggersTrigger> triggersAfter)
            {
                triggersBefore = new List<ITriggersTrigger>();
                triggersAfter = new List<ITriggersTrigger>();

                foreach (var trigger in _triggers)
                {
                    if (trigger.On?.Value == null || trigger.Time?.Value == null)
                    {
                        continue;
                    }

                    if (trigger.On.Value != onType || !IsTriggeredOnEachOrId(trigger, onId))
                    {
                        continue;
                    }

                    if (EnumTriggerTimeConverter.Convert(trigger.Time.Value) == EnumTriggerTime.Before)
                    {
                        triggersBefore.Add(trigger);
                    }
                    else if (EnumTriggerTimeConverter.Convert(trigger.Time.Value) == EnumTriggerTime.After)
                    {
                        triggersAfter.Add(trigger);
                    }
                }
            }

            private IReadable GetItem(ItemTypes type, uint id)
            {
                switch (type)
                {
                    case ItemTypes.Trigger:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.TriggersById, Convert.ToString(id), out ITriggersTrigger trigger))
                        {
                            return trigger;
                        }

                        break;

                    case ItemTypes.Action:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.ActionsById, Convert.ToString(id), out IActionsAction action))
                        {
                            return action;
                        }

                        break;

                    case ItemTypes.Group:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.GroupsById, Convert.ToString(id), out IGroupsGroup group))
                        {
                            return group;
                        }

                        break;

                    case ItemTypes.QAction:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.QActionsById, Convert.ToString(id), out IQActionsQAction qaction))
                        {
                            return qaction;
                        }

                        break;

                    case ItemTypes.Command:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.CommandsById, Convert.ToString(id), out ICommandsCommand command))
                        {
                            return command;
                        }

                        break;

                    case ItemTypes.Response:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.ResponsesById, Convert.ToString(id), out IResponsesResponse response))
                        {
                            return response;
                        }

                        break;

                    case ItemTypes.Pair:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.PairsById, Convert.ToString(id), out IPairsPair pair))
                        {
                            return pair;
                        }

                        break;

                    case ItemTypes.Param:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.ParamsById, Convert.ToString(id), out IParamsParam param))
                        {
                            return param;
                        }

                        break;

                    case ItemTypes.Timer:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.TimersById, Convert.ToString(id), out ITimersTimer timer))
                        {
                            return timer;
                        }

                        break;

                    case ItemTypes.Session:
                        if (_protocol.Model.TryGetObjectByKey(Mappings.SessionsById, Convert.ToString(id), out IHTTPSession session))
                        {
                            return session;
                        }

                        break;

                    default: return null;
                }

                return null;
            }

            private bool IsItemHandled(IReadable item)
            {
                return HandledItems.Contains(item);
            }

            private static bool IsTriggeredOnEachOrId(ITriggersTrigger trigger, uint id)
            {
                TriggerOnId onId = trigger.On.GetId();
                if (onId == null)
                {
                    return false;
                }

                return onId.Each || onId.Id == id;
            }

            private static uint[] GetAggregationTriggers(uint? defaultValue, ActionTypeOptions.ReturnClass returnValues, IReadOnlyCollection<(uint? columnIdx, uint? columnPid, bool shouldHavePid)> groupbyValues)
            {
                List<uint> ids = new List<uint>();

                if (defaultValue != null)
                {
                    ids.Add(defaultValue.Value);
                }

                if (returnValues != null)
                {
                    if (returnValues.Value1 != null)
                    {
                        ids.Add(returnValues.Value1.Value);
                    }

                    if (returnValues.Value2 != null)
                    {
                        ids.Add(returnValues.Value2.Value);
                    }

                    if (returnValues.Value3 != null)
                    {
                        ids.Add(returnValues.Value3.Value);
                    }

                    if (returnValues.Value4 != null)
                    {
                        ids.Add(returnValues.Value4.Value);
                    }
                }

                if (groupbyValues == null || groupbyValues.Count <= 0)
                {
                    return ids.ToArray();
                }

                foreach ((uint? _, uint? columnPid, bool _) in groupbyValues)
                {
                    if (columnPid != null)
                    {
                        ids.Add(columnPid.Value);
                    }
                }

                return ids.ToArray();
            }

            public void Clean()
            {
                Paths = new List<PathClass>();
                PathClass start = new PathClass();

                Paths.Add(start);
            }
            #endregion Methods
        }

        private sealed class PathClass
        {
            #region Constructor

            public PathClass()
            {
                Path = new List<Info>();
                LoopStartPositionInPath = -1;
            }

            public PathClass(IEnumerable<Info> path, bool hasCondition)
            {
                Path = path.Select(info => info.Clone() as Info).ToList();
                HasCondition = hasCondition;
            }

            #endregion Constructor

            #region Property

            public List<Info> Path { get; }

            private int LoopStartPositionInPath;

            public bool IsRunAction { get; set; }

            public bool HasCondition { get; set; }

            public bool IsActionSet { get; set; }

            #endregion Property

            #region Methods

            public string PathToString()
            {
                if (LoopStartPositionInPath > 0)
                {
                    Path.RemoveRange(0, LoopStartPositionInPath);
                }

                return String.Join("->", Path);
            }

            public bool ContainsItemInPath(ItemTypes type, uint id)
            {
                for (int i = 0; i < Path.Count; i++)
                {
                    if (Path[i].Id == id && Path[i].Type == type)
                    {
                        LoopStartPositionInPath = i;
                        return true;
                    }
                }

                return false;
            }

            #endregion Methods
        }

        private sealed class Info : ICloneable
        {
            public Info(uint id, ItemTypes type)
            {
                Id = id;
                Type = type;
            }

            public uint Id { get; }

            public ItemTypes Type { get; }

            public override string ToString()
            {
                return $"{Type} {Id}";
            }

            public object Clone()
            {
                Info info = new Info(Id, Type);
                return info;
            }
        }

        private enum ItemTypes
        {
            Param,
            Action,
            Group,
            Pair,
            Command,
            Response,
            Timer,
            QAction,
            Trigger,
            Session
        }
    }
}