<script lang="ts">
    import {
        ChartColumnOutline,
        EyeOutline,
        ShieldOutline,
        StarOutline,
        UsersOutline,
        ZapOutline,
    } from '~/lib/components/icons';

    const levels = [null, 'l1', 'l2', 'l3', 'l4'];
    const weights = [42, 24, 18, 11, 5];
    const total = 26 * 7;
    const levelClasses = {
        null: 'bg-base-subtle',
        l1: 'bg-pink-200 dark:bg-pink-900',
        l2: 'bg-pink-300 dark:bg-pink-800',
        l3: 'bg-pink-50 dark:bg-pink-700',
        l4: 'bg-pink-700 dark:bg-pink-500',
    };
    const cells = Array.from({ length: total }, pick);

    function pick() {
        const r = Math.random() * 100;
        let acc = 0;
        for (let i = 0; i < weights.length; i++) {
            acc += weights[i];
            if (r < acc) return levels[i];
        }
        return null;
    }
</script>

<section id="features" class="min-h-screen p-8">
    <div class="max-w-container-lg mx-auto">
        <div class="features-header">
            <p class="text-sm text-fg-muted uppercase tracking-wide font-light">Features</p>
            <h2 class="text-title-md mt-4 text-fg-emph">
                Everything you need to manage tasks effectively
            </h2>
            <p class="text-fg-dim">
                Powerful features designed to streamline your workflow and boost productivity.
            </p>
        </div>
        <div class="space-y-4 mt-8">
            <!-- Public by Default -->
            <div
                class="flex flex-col md:flex-row max-md:divide-y md:divide-x divide-surface-border border border-surface-border rounded-xl *:flex-1 overflow-auto"
            >
                <div class="p-8">
                    <div
                        class="w-fit bg-blue-50 dark:bg-blue-950 text-blue-500 border border-blue-100 dark:border-blue-900 p-1 rounded-lg"
                    >
                        <EyeOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                        Public by Default
                    </p>
                    <p class="mt-2 text-fg-dim">
                        Share your work as you build. Public progress creates awareness,
                        credibility, and recognition over time — turning your task list into a
                        living portfolio.
                    </p>
                </div>
                <div class="content-center bg-surface-subtle p-8">
                    <div class="grid grid-cols-[auto_1fr_auto] gap-2">
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 px-4 py-2 bg-surface shadow-sm rounded-lg items-center"
                        >
                            <div>
                                <div
                                    class="p-1 text-sm bg-blue-50 dark:bg-blue-950 text-blue-500 rounded-full text-center"
                                >
                                    JD
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Jordan D.</div>
                                <div class="text-fg-muted text-sm">
                                    completed <strong>API authentication flow</strong>
                                </div>
                            </div>
                            <div class="text-xs text-fg-muted">just now</div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 px-4 py-2 bg-surface shadow-sm rounded-lg items-center"
                        >
                            <div>
                                <div
                                    class="p-1 text-sm bg-pink-50 dark:bg-pink-950 text-pink-500 rounded-full text-center"
                                >
                                    AS
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Ava S.</div>
                                <div class="text-fg-muted text-sm">
                                    started <strong>Onboarding screen designs</strong>
                                </div>
                            </div>
                            <div class="text-xs text-fg-muted">2m ago</div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 px-4 py-2 bg-surface shadow-sm rounded-lg items-center"
                        >
                            <div>
                                <div
                                    class="p-1 text-sm bg-violet-50 dark:bg-violet-950 text-violet-500 rounded-full text-center"
                                >
                                    MK
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Maya K.</div>
                                <div class="text-fg-muted text-sm">
                                    opened <strong>Payment integration tests</strong>
                                </div>
                            </div>
                            <div class="text-xs text-fg-muted">5m ago</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Contributions (flipped) -->
            <div
                class="flex flex-col md:flex-row-reverse max-md:divide-y md:divide-x md:divide-x-reverse divide-surface-border border border-surface-border rounded-xl *:flex-1 overflow-auto"
            >
                <div class="p-8">
                    <div
                        class="w-fit bg-pink-50 dark:bg-pink-950 text-pink-500 border border-pink-100 dark:border-pink-900 p-1 rounded-lg"
                    >
                        <StarOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                        Contributions
                    </p>
                    <p class="mt-2 text-fg-dim">
                        Showcase your impact with a detailed contribution history — like a GitHub
                        graph, but for everything you build. Achievements and growth made visible.
                    </p>
                </div>
                <div class="content-center bg-surface-subtle p-8">
                    <div class="max-w-lg mx-auto">
                        <div class="text-sm text-fg-muted">Last 6 months · 184 contributions</div>
                        <div class="grid grid-cols-26 gap-1 mt-2">
                            {#each cells as level}
                                <div
                                    class="h-auto aspect-square rounded-xs {levelClasses[
                                        level as keyof typeof levelClasses
                                    ]}"
                                ></div>
                            {/each}
                        </div>
                    </div>
                </div>
            </div>

            <!-- Collaboration -->
            <div
                class="flex flex-col md:flex-row max-md:divide-y md:divide-x divide-surface-border border border-surface-border rounded-xl *:flex-1 overflow-auto"
            >
                <div class="p-8">
                    <div
                        class="w-fit bg-violet-50 dark:bg-violet-950 text-violet-500 border border-violet-100 dark:border-violet-900 p-1 rounded-lg"
                    >
                        <UsersOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                        Collaboration
                    </p>
                    <p class="mt-2 text-fg-dim">
                        Seamless collaboration tools that keep everyone aligned and productive, no
                        matter where they work. Assign, comment, and hand off tasks without leaving
                        your flow.
                    </p>
                </div>
                <div class="feature-visual bg-surface-subtle p-8">
                    <div class="grid grid-cols-[auto_1fr_auto] gap-2">
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 items-center px-4 py-2 bg-surface shadow-sm rounded-lg"
                        >
                            <div>
                                <div
                                    class="text-sm p-1 rounded-full bg-blue-50 dark:bg-blue-950 text-blue-500 text-center"
                                >
                                    JD
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Jordan D.</div>
                                <div class="text-fg-muted text-sm">API v2 — Auth flow</div>
                            </div>
                            <div class="text-right">
                                <span
                                    class="px-2 py-1 bg-green-50 dark:bg-green-950 text-green-500 rounded-md text-sm"
                                >
                                    Active
                                </span>
                            </div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 items-center px-4 py-2 bg-surface shadow-sm rounded-lg"
                        >
                            <div>
                                <div
                                    class="text-sm p-1 rounded-full bg-pink-50 dark:bg-pink-950 text-pink-500 text-center"
                                >
                                    AS
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Ava S.</div>
                                <div class="text-fg-muted text-sm">Mobile — Onboarding</div>
                            </div>
                            <div class="text-right">
                                <span
                                    class="px-2 py-1 bg-amber-50 dark:bg-amber-950 text-amber-500 rounded-md text-sm"
                                >
                                    In Review
                                </span>
                            </div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid gap-4 items-center px-4 py-2 bg-surface shadow-sm rounded-lg"
                        >
                            <div>
                                <div
                                    class="text-sm p-1 rounded-full bg-violet-50 dark:bg-violet-950 text-violet-500 text-center"
                                >
                                    MK
                                </div>
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium">Maya K.</div>
                                <div class="text-fg-muted text-sm">Payment tests</div>
                            </div>
                            <div class="text-right">
                                <span
                                    class="px-2 py-1 bg-violet-50 dark:bg-violet-950 text-violet-500 rounded-md text-sm"
                                >
                                    Todo
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div
                class="flex flex-col md:flex-row-reverse max-md:divide-y md:divide-x md:divide-x-reverse divide-surface-border border border-surface-border rounded-xl *:flex-1 overflow-auto"
            >
                <div class="p-8">
                    <div
                        class="w-fit bg-indigo-50 dark:bg-indigo-950 text-indigo-500 border border-indigo-100 dark:border-indigo-900 p-1 rounded-lg"
                    >
                        <ChartColumnOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                        Analytics
                    </p>
                    <p class="mt-2 text-fg-dim">
                        Gain insights into team performance and project health with comprehensive
                        reporting tools. Know what's moving and what's stuck at a glance.
                    </p>
                </div>
                <div class="bg-surface-subtle p-8 content-center">
                    <div class="relative">
                        <div
                            class="flex *:flex-1 shadow-sm rounded-lg divide-x divide-surface-border border border-surface-border"
                        >
                            <div class="p-4 bg-surface rounded-l-lg">
                                <div class="text-2xl font-bold">94</div>
                                <div class="text-sm text-fg-muted">Tasks done</div>
                                <div class="text-sm text-green-700 mt-2">↑ 12 this week</div>
                            </div>
                            <div class="p-4 bg-surface rounded-r-lg">
                                <div class="text-2xl font-bold">3.2d</div>
                                <div class="text-sm text-fg-muted">Avg cycle time</div>
                                <div class="text-sm text-green-700 mt-2">↓ 0.8d faster</div>
                            </div>
                        </div>
                        <svg
                            class="size-full absolute top-0 left-0 translate-y-4 pointer-events-none"
                            viewBox="0 0 260 40"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <defs
                                ><linearGradient id="sg" x1="0" y1="0" x2="0" y2="1"
                                    ><stop offset="0%" stop-color="rgba(21,138,69,0.1)" /><stop
                                        offset="100%"
                                        stop-color="rgba(21,138,69,0)"
                                    /></linearGradient
                                ></defs
                            >
                            <path
                                d="M0 35 C20 30, 40 28, 60 22 C80 16, 100 20, 120 15 C140 10, 160 18, 180 12 C200 6, 220 10, 260 5"
                                stroke="var(--color-green-500)"
                                stroke-opacity="0.2"
                                stroke-width="0.5"
                                stroke-linecap="round"
                            />
                            <path
                                d="M0 35 C20 30, 40 28, 60 22 C80 16, 100 20, 120 15 C140 10, 160 18, 180 12 C200 6, 220 10, 260 5 L260 40 L0 40Z"
                                fill="url(#sg)"
                            />
                        </svg>
                    </div>
                </div>
            </div>

            <!-- Fast + Security -->
            <div
                class="flex flex-col md:flex-row border border-surface-border rounded-xl *:flex-1 half-half max-md:divide-y md:divide-x divide-surface-border"
            >
                <div class="p-8">
                    <div
                        class="w-fit bg-sky-50 dark:bg-sky-950 text-sky-500 border border-sky-100 dark:border-sky-900 p-1 rounded-lg"
                    >
                        <ZapOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">Fast</p>
                    <p class="mt-2 text-fg-dim">
                        Optimized performance so your team can focus on what matters — getting
                        things done, not waiting on a spinner.
                    </p>
                    <div class="py-8">
                        <div class="grid grid-cols-[auto_1fr_auto] gap-2">
                            <div class="col-span-full grid grid-cols-subgrid items-center gap-4">
                                <span class="text-sm text-fg-muted font-light">Open task</span>
                                <div class="relative rounded-full h-2 bg-sky-100 dark:bg-sky-900">
                                    <div
                                        class="absolute inset-y-0 left-0 rounded-full bg-sky-500"
                                        style="width:18%;"
                                    ></div>
                                </div>
                                <span class="text-sm text-fg-muted">48ms</span>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center gap-4">
                                <span class="text-sm text-fg-muted font-light">Search</span>
                                <div class="relative rounded-full h-2 bg-sky-100 dark:bg-sky-900">
                                    <div
                                        class="absolute inset-y-0 left-0 rounded-full bg-sky-500"
                                        style="width:12%;"
                                    ></div>
                                </div>
                                <span class="text-sm text-fg-muted">30ms</span>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center gap-4">
                                <span class="text-sm text-fg-muted font-light">Save</span>
                                <div class="relative rounded-full h-2 bg-sky-100 dark:bg-sky-900">
                                    <div
                                        class="absolute inset-y-0 left-0 rounded-full bg-sky-500"
                                        style="width:8%;"
                                    ></div>
                                </div>
                                <span class="text-sm text-fg-muted">22ms</span>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center gap-4">
                                <span class="text-sm text-fg-muted font-light">Load page</span>
                                <div class="relative rounded-full h-2 bg-sky-100 dark:bg-sky-900">
                                    <div
                                        class="absolute inset-y-0 left-0 rounded-full bg-sky-500"
                                        style="width:25%;"
                                    ></div>
                                </div>
                                <span class="text-sm text-fg-muted">68ms</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="p-8 overflow-hidden">
                    <div
                        class="w-fit bg-green-50 dark:bg-green-950 text-green-500 border border-green-100 dark:border-green-900 p-1 rounded-lg"
                    >
                        <ShieldOutline class="size-8" />
                    </div>
                    <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                        Security
                    </p>
                    <p class="mt-2 text-fg-dim">
                        Fine-grained permissions and good security practices to keep your sensitive
                        data protected at every level.
                    </p>
                    <div class="py-8 max-md:pb-20">
                        <div class="relative w-fit mx-auto">
                            <div
                                class="-z-1 absolute translate-center size-64 border border-green-50 dark:border-green-950 rounded-full"
                            ></div>
                            <div
                                class="-z-1 absolute translate-center size-48 border border-green-100 dark:border-green-900 rounded-full"
                            ></div>
                            <div
                                class="-z-1 absolute translate-center size-36 border border-green-100 dark:border-green-900 rounded-full"
                            ></div>
                            <div
                                class="-z-1 absolute translate-center size-24 border border-green-200 dark:border-green-800 rounded-full"
                            ></div>
                            <div class="size-16">
                                <svg
                                    viewBox="0 0 22 22"
                                    fill="none"
                                    stroke="var(--color-green-500)"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                >
                                    <path
                                        d="M11 2L3.5 5.5v5.5c0 4.5 3.2 8 7.5 9 4.3-1 7.5-4.5 7.5-9V5.5L11 2z"
                                    />
                                    <path d="M7.5 11l2.5 2.5 5-5" />
                                </svg>
                            </div>
                            <span
                                class="absolute text-fg-dim text-nowrap bg-surface top-0 right-0 translate-x-full text-xs rounded-full border border-surface-border shadow-sm px-2 py-1"
                            >
                                Content ownership
                            </span>
                            <span
                                class="absolute text-fg-dim text-nowrap bg-surface bottom-0 left-0 -translate-x-full text-xs rounded-full border border-surface-border shadow-sm px-2 py-1"
                            >
                                Permission-based ACL
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Notifications full -->
            <div
                class="flex border border-surface-border rounded-xl *:flex-1 full divide-x divide-surface-border"
            >
                <div class="p-8 flex flex-col md:flex-row gap-4 *:flex-1">
                    <div>
                        <div
                            class="size-8 bg-amber-50 dark:bg-amber-950 text-amber-500 border border-amber-100 dark:border-amber-900 p-1 rounded-lg"
                        >
                            <svg
                                viewBox="0 0 18 18"
                                fill="none"
                                stroke="currentColor"
                                stroke-width="1.6"
                                stroke-linecap="round"
                                stroke-linejoin="round"
                            >
                                <path
                                    d="M9 2a5.5 5.5 0 015.5 5.5V11l1.5 2.5H2L3.5 11V7.5A5.5 5.5 0 019 2zM7 14.5a2 2 0 004 0"
                                />
                            </svg>
                        </div>
                        <p class="mt-4 font-semibold text-title-xs tracking-tight text-fg-emph">
                            Notifications
                        </p>
                        <p class="mt-2 text-fg-dim">
                            Stay informed without being overwhelmed. Intelligent notifications that
                            adapt to your workflow — you only hear about what actually matters.
                        </p>
                    </div>
                    <div class="grid grid-cols-[auto_1fr] gap-2">
                        <div
                            class="col-span-full grid grid-cols-subgrid px-4 py-2 border border-surface-border rounded-lg shadow-xs gap-4 items-center"
                        >
                            <div class="size-6 bg-blue-50 dark:bg-blue-950 p-1 rounded-md">
                                <svg
                                    viewBox="0 0 13 13"
                                    fill="none"
                                    stroke="var(--color-blue-500)"
                                    stroke-width="1.5"
                                    stroke-linecap="round"
                                    ><path d="M1.5 6.5h10M6.5 2l4.5 4.5L6.5 11" /></svg
                                >
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium text-sm">
                                    Jordan assigned you a task
                                </div>
                                <div class="text-sm text-fg-muted">
                                    API v2 — Auth flow · due in 2 days
                                </div>
                            </div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid px-4 py-2 border border-surface-border rounded-lg shadow-xs gap-4 items-center"
                        >
                            <div class="size-6 bg-green-50 dark:bg-green-950 p-1 rounded-md">
                                <svg
                                    viewBox="0 0 13 13"
                                    fill="none"
                                    stroke="var(--color-green-500)"
                                    stroke-width="1.5"
                                    stroke-linecap="round"><path d="M2 7l3 3 6-6" /></svg
                                >
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium text-sm">
                                    Ava completed Homepage redesign
                                </div>
                                <div class="text-sm text-fg-muted">
                                    Website Redesign project · just now
                                </div>
                            </div>
                        </div>
                        <div
                            class="col-span-full grid grid-cols-subgrid px-4 py-2 border border-surface-border rounded-lg shadow-xs gap-4 items-center"
                            style="opacity:0.5"
                        >
                            <div class="size-6 bg-amber-50 dark:bg-amber-950 p-1 rounded-md">
                                <svg
                                    viewBox="0 0 13 13"
                                    fill="none"
                                    stroke="var(--color-amber-500)"
                                    stroke-width="1.5"
                                    stroke-linecap="round"
                                    ><circle cx="6.5" cy="6.5" r="5" /><path
                                        d="M6.5 4v3l1.5 1.5"
                                    /></svg
                                >
                            </div>
                            <div>
                                <div class="text-fg-emph font-medium text-sm">
                                    Deadline tomorrow
                                </div>
                                <div class="text-sm text-fg-muted">
                                    Lighthouse audit · Website Redesign
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
