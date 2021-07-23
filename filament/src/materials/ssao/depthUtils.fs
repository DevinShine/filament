/*
 * Copyright (C) 2021 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#ifndef FILAMENT_MATERIALS_DEPTH_UTILS
#define FILAMENT_MATERIALS_DEPTH_UTILS

highp float linearizeDepth(highp float depth, highp float depthParams) {
    // Our far plane is at infinity, which causes a division by zero below, which in turn
    // causes some issues on some GPU. We workaround it by replacing "infinity" by the closest
    // value representable in  a 24 bit depth buffer.
    const float preventDiv0 = 1.0 / 16777216.0;
    return depthParams / max(depth, preventDiv0);
}

highp float sampleDepth(const highp sampler2D depthTexture, const highp vec2 uv, float lod) {
#if defined(TARGET_METAL_ENVIRONMENT) || defined(TARGET_VULKAN_ENVIRONMENT)
    // On metal/vulkan, texture space is flipped vertically and we need to adjust the uv
    // coordinates.
    return textureLod(depthTexture, vec2(uv.x, 1.0 - uv.y), lod).r;
#else
    return textureLod(depthTexture, uv, lod).r;
#endif
}

highp float sampleDepthLinear(const highp sampler2D depthTexture,
        const highp vec2 uv, float lod, highp float depthParams) {
    return linearizeDepth(sampleDepth(depthTexture, uv, lod), depthParams);
}

#endif // #define FILAMENT_MATERIALS_DEPTH_UTILS

