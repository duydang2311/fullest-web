<script lang="ts">
    import {
        ChartColumnOutline,
        EyeOutline,
        MessageSquareOutline,
        ShieldOutline,
        StarOutline,
        UsersOutline,
        ZapOutline,
    } from '~/lib/components/icons';

    const features = [
        {
            icon: EyeOutline,
            title: 'Public by Default',
            description:
                'Share your work as you build. Public progress creates awareness, credibility, and recognition over time.',
        },
        {
            icon: StarOutline,
            title: 'Contributions',
            description:
                'Showcase your impact with a detailed contribution history that highlights your achievements and growth over time.',
        },
        {
            icon: UsersOutline,
            title: 'Collaboration',
            description:
                'Seamless collaboration tools that keep everyone aligned and productive, no matter where they work.',
        },
        {
            icon: ZapOutline,
            title: 'Fast',
            description:
                'Optimized performance ensures your team can focus on what matters most - getting things done.',
        },
        {
            icon: ShieldOutline,
            title: 'Security',
            description:
                'Good security with granular permissions to keep your sensitive data protected.',
        },
        {
            icon: ChartColumnOutline,
            title: 'Analytics',
            description:
                'Gain insights into team performance and project health with comprehensive reporting tools.',
        },
        {
            icon: MessageSquareOutline,
            title: 'Notifications',
            description:
                'Stay informed without being overwhelmed. Intelligent notifications that adapt to your workflow.',
        },
    ];
</script>

<section id="features" class="bg-base text-base-fg relative min-h-screen p-8" data-theme="accent">
    <svg
        viewBox="0 0 900 600"
        xmlns="http://www.w3.org/2000/svg"
        class="absolute inset-x-0 w-full top-1 -translate-y-[97%] -z-10 text-base/4"
    >
        <path
            d="M0 526L25 531.5C50 537 100 548 150 551.2C200 554.3 250 549.7 300 546C350 542.3 400 539.7 450 543C500 546.3 550 555.7 600 554.2C650 552.7 700 540.3 750 534.3C800 528.3 850 528.7 875 528.8L900 529L900 601L875 601C850 601 800 601 750 601C700 601 650 601 600 601C550 601 500 601 450 601C400 601 350 601 300 601C250 601 200 601 150 601C100 601 50 601 25 601L0 601Z"
            fill="currentColor"
            stroke-linecap="round"
            stroke-linejoin="miter"
        >
        </path>
    </svg>
    <svg
        viewBox="0 0 900 600"
        xmlns="http://www.w3.org/2000/svg"
        class="absolute inset-x-0 w-full top-1 -translate-y-[95%] -z-10 text-base/10"
    >
        <path
            d="M0 526L25 531.5C50 537 100 548 150 551.2C200 554.3 250 549.7 300 546C350 542.3 400 539.7 450 543C500 546.3 550 555.7 600 554.2C650 552.7 700 540.3 750 534.3C800 528.3 850 528.7 875 528.8L900 529L900 601L875 601C850 601 800 601 750 601C700 601 650 601 600 601C550 601 500 601 450 601C400 601 350 601 300 601C250 601 200 601 150 601C100 601 50 601 25 601L0 601Z"
            fill="currentColor"
            stroke-linecap="round"
            stroke-linejoin="miter"
        >
        </path></svg
    >
    <svg
        viewBox="0 0 900 600"
        xmlns="http://www.w3.org/2000/svg"
        class="absolute inset-x-0 w-full top-1 -translate-y-[94%] -z-10 text-base"
    >
        <path
            d="M0 559L25 557.2C50 555.3 100 551.7 150 545C200 538.3 250 528.7 300 526.7C350 524.7 400 530.3 450 535.5C500 540.7 550 545.3 600 548.2C650 551 700 552 750 548.2C800 544.3 850 535.7 875 531.3L900 527L900 601L875 601C850 601 800 601 750 601C700 601 650 601 600 601C550 601 500 601 450 601C400 601 350 601 300 601C250 601 200 601 150 601C100 601 50 601 25 601L0 601Z"
            fill="currentColor"
            stroke-linecap="round"
            stroke-linejoin="miter"
        ></path>
    </svg>
    <div class="max-w-container-xl mx-auto mt-4">
        <h2 class="text-base-fg-strong font-medium leading-none tracking-tighter text-title-md">
            Everything you need to manage tasks effectively
        </h2>
        <p class="mt-4">
            Powerful features designed to streamline your workflow and boost productivity.
        </p>
    </div>
    <div class="max-w-container-xl mx-auto mt-8 grid grid-cols-1 xl:grid-cols-3 gap-4">
        <div
            class="xl:col-span-2 relative border-blue-500/20 hover:border-blue-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div>
                <h3 class="text-base-fg-strong font-medium mt-2 text-title-sm">
                    {features[0].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[0].description}</p>
            </div>
            <div
                class="absolute size-56 bg-blue-500/10 top-0 right-0 translate-x-1/2 -translate-y-1/2 rounded-full"
            ></div>
            <div
                class="relative flex items-center justify-center rounded-lg p-8 bg-blue-500/5 border border-blue-500/10 rotate-3"
            >
                <EyeOutline class="relative text-blue-500 size-16" />
            </div>
        </div>
        <div
            class="xl:col-span-1 relative border-pink-500/20 hover:border-pink-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div>
                <div
                    class="size-12 relative flex items-center justify-center rounded-lg p-2 bg-pink-500/5 border border-pink-500/20 rotate-3"
                >
                    <StarOutline class="relative text-pink-500 size-full" />
                </div>
                <h3 class="text-base-fg-strong font-medium mt-4 text-title-sm">
                    {features[1].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[1].description}</p>
            </div>
            <div
                class="absolute size-56 bg-pink-500/10 top-0 right-0 translate-x-2/3 -translate-y-2/3 rounded-lg -rotate-12"
            ></div>
        </div>
        <div
            class="xl:col-span-1 relative border-fuchsia-500/20 hover:border-fuchsia-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div
                class="absolute size-56 bg-fuchsia-500/10 top-0 left-0 -translate-x-1/2 -translate-y-1/2 rounded-full"
            ></div>
            <div
                class="absolute w-full aspect-square bg-fuchsia-500/10 bottom-0 right-0 translate-x-1/2 translate-y-3/4 rounded-full"
            ></div>
            <div class="relative">
                <div class="flex gap-2 *:-ml-4 pl-4">
                    <div
                        class="size-10 rounded-full p-1 bg-fuchsia-500/20 border-4 border-base flex items-center justify-center text-base-fg-strong"
                    >
                        JD
                    </div>
                    <div
                        class="size-10 rounded-full p-1 bg-pink-500/20 border-4 border-base flex items-center justify-center text-base-fg-strong"
                    >
                        AS
                    </div>
                    <div
                        class="size-10 rounded-full p-1 bg-sky-500/20 border-4 border-base flex items-center justify-center text-base-fg-strong"
                    >
                        +3
                    </div>
                </div>
                <h3 class="text-base-fg-strong font-medium text-title-sm mt-4">
                    {features[2].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[2].description}</p>
            </div>
        </div>
        <div
            class="xl:col-span-2 relative border-sky-500/20 hover:border-sky-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div>
                <h3 class="text-base-fg-strong font-medium mt-2 text-title-sm">
                    {features[3].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[3].description}</p>
            </div>
            <div
                class="absolute size-56 bg-sky-500/10 top-0 right-0 translate-x-1/2 -translate-y-1/2 rounded-full"
            ></div>
            <div
                class="relative flex items-center justify-center rounded-lg p-8 bg-sky-500/5 border border-sky-500/10 rotate-3"
            >
                <ZapOutline class="relative text-sky-500 size-16" />
            </div>
        </div>
        <div
            class="xl:col-span-1 relative border-green-500/20 hover:border-green-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div
                class="absolute size-56 bg-green-500/10 top-0 right-0 translate-x-2/3 -translate-y-2/3 rounded-lg -rotate-12"
            ></div>
            <div class="relative">
                <div
                    class="size-12 relative flex items-center justify-center rounded-lg p-2 bg-green-500/5 border border-green-500/20 rotate-3"
                >
                    <ShieldOutline class="relative text-green-500 size-full" />
                </div>
                <h3 class="text-base-fg-strong font-medium mt-2 text-title-sm">
                    {features[4].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[4].description}</p>
            </div>
        </div>
        <div
            class="xl:col-span-2 relative border-indigo-500/20 hover:border-indigo-500/30 rounded-2xl border p-6 transition hover:shadow-sm flex justify-between items-center gap-8 overflow-hidden"
        >
            <div
                class="absolute size-56 bg-indigo-500/10 top-0 right-0 translate-x-2/3 -translate-y-2/3 rounded-lg -rotate-12"
            ></div>
            <div class="relative">
                <div
                    class="size-12 flex items-center justify-center rounded-lg p-2 bg-indigo-500/5 border border-indigo-500/20 rotate-3"
                >
                    <ChartColumnOutline class="text-indigo-500 size-full" />
                </div>
                <h3 class="text-base-fg-strong font-medium mt-4 text-title-sm">
                    {features[5].title}
                </h3>
                <p class="mt-2 text-pretty max-w-[80ch]">{features[5].description}</p>
            </div>
            <div class="relative flex items-end gap-2 h-full border-b border-b-indigo-500/20">
                <div class="w-8 h-1/2 bg-indigo-500/40 rounded-t"></div>
                <div class="w-8 h-1/4 bg-indigo-500/40 rounded-t"></div>
                <div class="w-8 h-1/5 bg-indigo-500/40 rounded-t"></div>
                <div class="w-8 h-2/5 bg-indigo-500/40 rounded-t"></div>
                <div class="w-8 h-full bg-indigo-500 rounded-t"></div>
            </div>
        </div>
        <!-- {#each features as feature (feature.title)}
            <div
                class="relative border-base-border hover:border-base-border-strong rounded-2xl border p-6 transition hover:shadow-sm flex items-center gap-8 overflow-hidden"
            >
                <div>
                    <h3 class="text-base-fg-strong font-medium mt-2 text-title-sm">
                        {feature.title}
                    </h3>
                    <p class="mt-2 text-pretty">{feature.description}</p>
                </div>
                <div
                    class="absolute size-56 bg-blue-500/10 top-0 right-0 translate-x-1/2 translate-y-(--_y) rounded-full"
                ></div>
                <div
                    class="relative flex items-center justify-center rounded-lg p-8 bg-blue-500/5 border border-blue-500/10 rotate-3"
                >
                    <feature.icon class="relative text-blue-500 size-16" />
                </div>
            </div>
        {/each} -->
    </div>
</section>
