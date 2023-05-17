// using System.Collections.Generic;
// using System.Linq;
// using Applitools.Playwright.Universal.Dto;
// using Applitools.Universal;
// using Microsoft.Playwright;
//
// namespace Applitools.Playwright.Universal.Mapper
// {
//     public class TFramesMapper
//     {
//         public static ContextReferenceDto ToContextReferenceDto(FrameLocator frame, Refer refer, Reference root)
//         {
//             if (frame == null)
//             {
//                 return null;
//             }
//
//             ContextReferenceDto contextReferenceDto = new ContextReferenceDto();
//
//             string frameNameOrId = frame.FrameNameOrId;
//             if (!string.IsNullOrEmpty(frameNameOrId))
//             {
//                 contextReferenceDto.setFrame(frameNameOrId);
//             }
//
//             int? frameIndex = frame.FrameIndex;
//             if (frameIndex != null)
//             {
//                 contextReferenceDto.setFrame(frameIndex);
//             }
//
//             Element frameElement = frame.FrameElement;
//             if (frameElement != null)
//             {
//                 IElementHandle elementHandle = frameElement.ElementHandle;
//                 frameElement.ApplitoolsRefId = refer.Ref(elementHandle, root);
//                 contextReferenceDto.setFrame(frameElement);
//             }
//
//             Selector frameSelector = frame.FrameSelector;
//             if (frameSelector != null)
//             {
//                 if (frameSelector.Locator != null)
//                 {
//                     ILocator locator = frameSelector.Locator;
//                     frameSelector.ApplitoolsRefId = refer.Ref(locator, root);
//                 }
//
//                 contextReferenceDto.setFrame(frameSelector);
//             }
//
//             Selector scrollSelector = frame.ScrollRootSelector;
//             if (scrollSelector != null)
//             {
//                 if (scrollSelector.Locator != null)
//                 {
//                     ILocator locator = scrollSelector.Locator;
//                     scrollSelector.ApplitoolsRefId = refer.Ref(locator, root);
//                 }
//
//                 contextReferenceDto.setScrollRootElement(scrollSelector);
//             }
//
//             Element scrollElement = frame.ScrollRootElement;
//             if (scrollElement != null)
//             {
//                 IElementHandle elementHandle = scrollElement.ElementHandle;
//                 scrollElement.ApplitoolsRefId = refer.Ref(elementHandle, root);
//                 contextReferenceDto.setScrollRootElement(scrollElement);
//             }
//
//             return contextReferenceDto;
//         }
//
//         public static List<ContextReferenceDto> ToTFramesFromCheckSettings(List<FrameLocator> frameChain, Refer refer,
//             Reference root)
//         {
//             if (frameChain == null || frameChain.Count == 0)
//             {
//                 return null;
//             }
//
//             frameChain.Select(frame => ToContextReferenceDto(frame, refer, root)).ToList();
//         }
//     }
// }