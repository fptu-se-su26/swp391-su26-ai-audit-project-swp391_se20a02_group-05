"use client";

import React, { useState, useRef } from "react";
import { useParams } from "next/navigation";
import { Card } from "@/components/ui/card";
import { Typography, Chip, toast } from "@heroui/react";
import { useWorkspaceStore } from "@/features/workspace/store/use-workspace-store";
import { useAuthStore } from "@/features/auth/store/use-auth-store";
import {
  ThumbsUp,
  MessageSquare,
  Share2,
  Globe,
  MoreHorizontal,
  Send,
  Heart,
  Check,
  X,
  Plus
} from "lucide-react";

interface Comment {
  id: string;
  authorName: string;
  authorAvatar?: string;
  content: string;
  date: string;
}

interface Post {
  id: string;
  category: "Announcement" | "Engineering" | "Recruitment";
  author: string;
  authorRole: string;
  date: string;
  content: string;
  images: string[];
  likes: number;
  comments: Comment[];
  sharesCount: number;
}

export default function WorkspacePostsTab() {
  const params = useParams();
  const organizationSlug = typeof params?.organizationSlug === "string" ? params.organizationSlug : "";

  const workspaceDetails = useWorkspaceStore((s) => s.workspaces[organizationSlug]);
  const user = useAuthStore((s) => s.user);

  // Mock Posts with realistic Unsplash corporate/technical image arrays
  const [posts, setPosts] = useState<Post[]>([
    {
      id: "post-1",
      category: "Engineering",
      author: "Hoang Nguyen",
      authorRole: "Tech Lead",
      date: "12 May at 10:14",
      content: "Chúng tôi vô cùng tự hào thông báo rằng quy trình đánh giá và xác thực lập trình viên trên CVerify đã chính thức tích hợp chữ ký mật mã hóa (cryptographic credential signatures)! Việc này giúp tự động hóa 100% quy trình kiểm thử năng lực thực tế từ kho lưu trữ mã nguồn của ứng viên.\n\nĐặc biệt, đại diện CVerify cùng đối tác đã ký kết biên bản ghi nhớ hợp tác chiến lược nhằm xây dựng cộng đồng kỹ sư công nghệ chất lượng cao, bảo mật và đáng tin cậy. Dưới đây là một số hình ảnh sự kiện ký kết và hoạt động triển khai thực tế của đội ngũ kỹ sư tại văn phòng Đà Nẵng.",
      images: [
        "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?q=80&w=800",
        "https://images.unsplash.com/photo-1531538606174-0f90ff5dce83?q=80&w=800",
        "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?q=80&w=800",
        "https://images.unsplash.com/photo-1522071820081-009f0129c71c?q=80&w=800",
        "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?q=80&w=800",
        "https://images.unsplash.com/photo-1497366216548-37526070297c?q=80&w=800"
      ],
      likes: 88,
      sharesCount: 14,
      comments: [
        {
          id: "c-1",
          authorName: "Nguyễn Hoàng Ngọc Ánh",
          content: "Sự kiện ký kết hoành tráng quá ạ! Chúc mừng CVerify và đối tác.",
          date: "2h ago"
        },
        {
          id: "c-2",
          authorName: "Lê Minh",
          content: "Great progress, keep up the outstanding work team! Tính năng mới rất thực tế.",
          date: "1h ago"
        },
        {
          id: "c-3",
          authorName: "Trần Tuấn",
          content: "Xác thực repo github chạy cực kỳ nhanh và mượt nha mọi người.",
          date: "30m ago"
        }
      ]
    },
    {
      id: "post-2",
      category: "Recruitment",
      author: "Trang Pham",
      authorRole: "HR Lead",
      date: "9 May at 15:30",
      content: "WE ARE HIRING! GIA NHẬP ĐỘI NGŨ CÔNG NGHỆ CỦA CHÚNG TÔI.\n\nNhằm mở rộng quy mô dự án và đáp ứng nhu cầu tăng trưởng trong giai đoạn mới, chúng tôi tìm kiếm các đồng nghiệp tài năng ở các vị trí:\n1. Senior Full-Stack Developer (.NET & React)\n2. Automated QA Engineer\n3. DevOps Engineer (Platform Team)\n\nChúng tôi mang đến môi trường làm việc Hybrid linh hoạt, chế độ đãi ngộ cạnh tranh, hỗ trợ thiết bị làm việc hiện đại hàng đầu cùng cơ hội phát triển bản thân vượt trội. Hãy truy cập ngay tab 'Jobs' để xem chi tiết mô tả công việc và ứng tuyển trực tiếp bằng hồ sơ đã xác thực nhé!",
      images: [
        "https://images.unsplash.com/photo-1521737711867-e3b90473bd58?q=80&w=800"
      ],
      likes: 42,
      sharesCount: 5,
      comments: [
        {
          id: "c-4",
          authorName: "Trần Quốc Bảo",
          content: "Bên mình có nhận Intern Web (.NET/React) chưa tốt nghiệp không chị ơi?",
          date: "2d ago"
        },
        {
          id: "c-5",
          authorName: "Trang Pham",
          content: "@Trần Quốc Bảo Có em nhé, bên mình đang mở đợt tuyển thực tập sinh có lương. Em cứ gửi CV đã xác thực qua hệ thống nha.",
          date: "1d ago"
        }
      ]
    },
    {
      id: "post-3",
      category: "Announcement",
      author: "Minh Le",
      authorRole: "CEO",
      date: "3 May at 09:00",
      content: "Chính thức đạt chứng nhận doanh nghiệp uy tín cấp độ Level 3 (Verified Enterprise status) trên cổng CVerify!\n\nChúng tôi đã hoàn thành các bước kiểm tra nghiêm ngặt về quyền sở hữu tên miền, giấy phép đăng ký kinh doanh và chữ ký số của đại diện pháp luật. Mọi thông tin cốt lõi đều được hashing mật mã hóa và ghi nhận tin cậy. Đây là cột mốc khẳng định cam kết tuyệt đối về tính minh bạch và uy tín công nghệ của doanh nghiệp đối với mọi khách hàng và đối tác.",
      images: [
        "https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?q=80&w=800",
        "https://images.unsplash.com/photo-1556761175-b413da4baf72?q=80&w=800",
        "https://images.unsplash.com/photo-1497215728101-856f4ea42174?q=80&w=800"
      ],
      likes: 56,
      sharesCount: 8,
      comments: [
        {
          id: "c-6",
          authorName: "Hoàng Nguyễn",
          content: "Cột mốc rất ý nghĩa! Tự hào là một phần của hành trình công nghệ này.",
          date: "1w ago"
        }
      ]
    }
  ]);

  const [likedPosts, setLikedPosts] = useState<string[]>([]);
  const [expandedPosts, setExpandedPosts] = useState<string[]>([]);
  const [commentInputs, setCommentInputs] = useState<Record<string, string>>({});
  const [showAllComments, setShowAllComments] = useState<Record<string, boolean>>({});

  const commentInputRefs = useRef<Record<string, HTMLInputElement | null>>({});

  const [showCreatePostModal, setShowCreatePostModal] = useState(false);
  const [newPostContent, setNewPostContent] = useState("");
  const [newPostCategory, setNewPostCategory] = useState<"Announcement" | "Engineering" | "Recruitment">("Announcement");
  const [newPostImages, setNewPostImages] = useState("");

  if (!workspaceDetails) return null;

  // Reactive Permission helper key check
  const hasPermission = (permissionKey: string): boolean => {
    if (!workspaceDetails) return false;
    // Fallback logic for managers
    if (
      workspaceDetails.userRole === "OWNER" ||
      workspaceDetails.userRole === "REPRESENTATIVE" ||
      workspaceDetails.userRole === "HR"
    ) {
      return true;
    }
    return workspaceDetails.permissions?.includes(permissionKey) || false;
  };

  // Handle local form submission for post creation
  const handleCreatePostSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newPostContent.trim()) {
      toast.danger("Vui lòng nhập nội dung thông báo!");
      return;
    }

    const created: Post = {
      id: `post-new-${Date.now()}`,
      category: newPostCategory,
      author: user?.fullName || "Manager",
      authorRole: workspaceDetails.userRole || "Administrator",
      date: "Just now",
      content: newPostContent.trim(),
      images: newPostImages.trim()
        ? newPostImages.split(",").map((s) => s.trim()).filter(Boolean)
        : [],
      likes: 0,
      sharesCount: 0,
      comments: []
    };

    setPosts([created, ...posts]);
    toast.success("Đăng thông báo thành công!");

    // Reset fields
    setNewPostContent("");
    setNewPostImages("");
    setNewPostCategory("Announcement");
    setShowCreatePostModal(false);
  };

  // Toggle like status (Facebook style)
  const handleLike = (postId: string) => {
    if (likedPosts.includes(postId)) {
      setLikedPosts(likedPosts.filter((id) => id !== postId));
      setPosts(
        posts.map((p) => (p.id === postId ? { ...p, likes: p.likes - 1 } : p))
      );
    } else {
      setLikedPosts([...likedPosts, postId]);
      setPosts(
        posts.map((p) => (p.id === postId ? { ...p, likes: p.likes + 1 } : p))
      );
    }
  };

  // Focus comment input field
  const handleFocusCommentInput = (postId: string) => {
    const inputEl = commentInputRefs.current[postId];
    if (inputEl) {
      inputEl.focus();
    }
  };

  // Handle share post link
  const handleSharePost = (postId: string) => {
    if (typeof window !== "undefined") {
      const shareUrl = `${window.location.origin}/workspace/${organizationSlug}/posts?post=${postId}`;
      navigator.clipboard.writeText(shareUrl);
      toast.success("Đã sao chép liên kết bài viết vào bộ nhớ tạm!");
    }
  };

  // Handle text input for comments
  const handleCommentInputChange = (postId: string, text: string) => {
    setCommentInputs({
      ...commentInputs,
      [postId]: text
    });
  };

  // Submit comment
  const handleSubmitComment = (postId: string) => {
    const content = commentInputs[postId]?.trim();
    if (!content) return;

    const newComment: Comment = {
      id: `c-new-${Date.now()}`,
      authorName: user?.fullName || "Khách ghé thăm",
      authorAvatar: user?.avatarUrl,
      content,
      date: "Just now"
    };

    setPosts(
      posts.map((p) =>
        p.id === postId
          ? {
              ...p,
              comments: [...p.comments, newComment]
            }
          : p
      )
    );

    // Clear input
    setCommentInputs({
      ...commentInputs,
      [postId]: ""
    });

    // Auto expand to show the newly added comment
    setShowAllComments({
      ...showAllComments,
      [postId]: true
    });
  };

  // Component for image grid layout (Facebook collage grid)
  const PostImageGrid = ({ images }: { images: string[] }) => {
    if (!images || images.length === 0) return null;

    if (images.length === 1) {
      return (
        <div className="overflow-hidden border border-border/60 bg-card/20 rounded-md">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[0]} alt="Post attachment" className="w-full h-auto max-h-[480px] object-cover" />
        </div>
      );
    }

    if (images.length === 2) {
      return (
        <div className="grid grid-cols-2 gap-1 overflow-hidden border border-border/60 bg-card/20 rounded-md h-[280px]">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[0]} alt="Post attachment 1" className="w-full h-full object-cover" />
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[1]} alt="Post attachment 2" className="w-full h-full object-cover" />
        </div>
      );
    }

    if (images.length === 3) {
      return (
        <div className="grid grid-cols-2 gap-1 overflow-hidden border border-border/60 bg-card/20 rounded-md h-[360px]">
          <div className="col-span-1 h-full">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={images[0]} alt="Post attachment 1" className="w-full h-full object-cover" />
          </div>
          <div className="col-span-1 grid grid-rows-2 gap-1 h-full">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={images[1]} alt="Post attachment 2" className="w-full h-full object-cover" />
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={images[2]} alt="Post attachment 3" className="w-full h-full object-cover" />
          </div>
        </div>
      );
    }

    if (images.length === 4) {
      return (
        <div className="grid grid-cols-2 gap-1 overflow-hidden border border-border/60 bg-card/20 rounded-md h-[360px]">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[0]} alt="Post attachment 1" className="w-full h-full object-cover" />
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[1]} alt="Post attachment 2" className="w-full h-full object-cover" />
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[2]} alt="Post attachment 3" className="w-full h-full object-cover" />
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[3]} alt="Post attachment 4" className="w-full h-full object-cover" />
        </div>
      );
    }

    // 5 or more images
    return (
      <div className="grid grid-cols-6 gap-1 overflow-hidden border border-border/60 bg-card/20 rounded-md h-[380px]">
        <div className="col-span-3 h-52">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[0]} alt="Post attachment 1" className="w-full h-full object-cover" />
        </div>
        <div className="col-span-3 h-52">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[1]} alt="Post attachment 2" className="w-full h-full object-cover" />
        </div>
        <div className="col-span-2 h-36">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[2]} alt="Post attachment 3" className="w-full h-full object-cover" />
        </div>
        <div className="col-span-2 h-36">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[3]} alt="Post attachment 4" className="w-full h-full object-cover" />
        </div>
        <div className="col-span-2 h-36 relative select-none">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img src={images[4]} alt="Post attachment 5" className="w-full h-full object-cover" />
          {images.length > 5 && (
            <div className="absolute inset-0 bg-black/60 flex items-center justify-center text-white text-base font-bold">
              +{images.length - 4}
            </div>
          )}
        </div>
      </div>
    );
  };

  const orgName = workspaceDetails.organizationName || "Doanh nghiệp đối tác";
  const orgLogo = workspaceDetails.logoUrl;

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
      {/* ── Main Feed Column ── */}
      <div className="lg:col-span-2 space-y-6">
        {/* "What's on your mind?" announcement widget */}
        {hasPermission("organization:posts:write") && (
          <Card className="p-4 bg-surface border border-border rounded-xl flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-accent/10 border border-border flex items-center justify-center text-accent font-semibold text-sm shrink-0 select-none overflow-hidden">
              {user?.avatarUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img src={user.avatarUrl} alt={user.fullName} className="w-full h-full object-cover" />
              ) : (
                user?.fullName?.substring(0, 1).toUpperCase() || "M"
              )}
            </div>
            <button
              onClick={() => setShowCreatePostModal(true)}
              className="flex-1 text-left bg-card/40 hover:bg-card/60 transition-colors border border-border rounded-full px-4 py-2.5 text-muted-foreground text-xs cursor-pointer font-outfit"
            >
              Write an announcement or share company updates...
            </button>
          </Card>
        )}

        {posts.map((post) => {
          const isLiked = likedPosts.includes(post.id);

          // Truncate logic
          const isLong = post.content.length > 250;
          const shouldTruncate = isLong && !expandedPosts.includes(post.id);
          const displayText = shouldTruncate ? `${post.content.slice(0, 250)}...` : post.content;

          // Comments list visibility logic
          const commentsList = post.comments;
          const hasManyComments = commentsList.length > 3;
          const showingAll = showAllComments[post.id];
          const visibleComments = showingAll ? commentsList : commentsList.slice(0, 3);

          return (
            <Card key={post.id} className="p-4 bg-surface border border-border rounded-xl space-y-4">
              {/* ── Facebook Post Header ── */}
              <div className="flex justify-between items-center">
                <div className="flex items-center gap-3">
                  {/* Organization Avatar */}
                  <div className="w-10 h-10 rounded-full bg-accent/10 border border-border flex items-center justify-center text-accent font-semibold text-sm select-none overflow-hidden shrink-0">
                    {orgLogo ? (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img src={orgLogo} alt={`${orgName} Logo`} className="w-full h-full object-cover" />
                    ) : (
                      orgName.substring(0, 1).toUpperCase()
                    )}
                  </div>

                  <div>
                    <div className="flex items-center gap-1.5 flex-wrap">
                      <Typography type="body-sm" className="font-semibold text-foreground text-sm hover:underline cursor-pointer">
                        {orgName}
                      </Typography>
                      {/* Blue Verified Checkmark */}
                      <span
                        title="Verified Enterprise"
                        className="inline-flex items-center justify-center bg-blue-500 rounded-full p-0.5 shrink-0 text-white size-3.5 select-none"
                      >
                        <Check className="size-2" strokeWidth={4} />
                      </span>
                    </div>

                    {/* Subtitle with date & Globe icon */}
                    <div className="flex items-center gap-1 text-[11px] text-muted font-normal select-none">
                      <span>{post.date}</span>
                      <span>·</span>
                      <span title="Public"><Globe className="size-3" /></span>
                    </div>
                  </div>
                </div>

                {/* Tag + Menu icon */}
                <div className="flex items-center gap-2 select-none">
                  <Chip size="sm" variant="soft" color="default" className="text-[9px] font-medium h-5 px-1.5">
                    {post.category}
                  </Chip>
                  <button
                    aria-label="Settings"
                    className="text-muted hover:text-foreground p-1.5 rounded-full hover:bg-card/50 transition-colors cursor-pointer border-none"
                  >
                    <MoreHorizontal className="size-5" />
                  </button>
                </div>
              </div>

              {/* ── Facebook Post Content ── */}
              <div className="space-y-3 font-normal text-xs leading-relaxed">
                <Typography type="body-xs" className="text-foreground whitespace-pre-line text-xs font-normal">
                  {displayText}
                  {shouldTruncate && (
                    <button
                      onClick={() => setExpandedPosts([...expandedPosts, post.id])}
                      className="text-accent font-semibold hover:underline text-xs ml-1 focus:outline-hidden cursor-pointer"
                    >
                      Xem thêm
                  </button>
                  )}
                </Typography>

                {/* Collage Grid */}
                <PostImageGrid images={post.images} />
              </div>

              {/* ── Facebook Interactions Counter ── */}
              <div className="flex items-center justify-between text-[11px] text-muted select-none pb-1 font-normal">
                <div className="flex items-center gap-1.5">
                  <span className="flex items-center">
                    <span className="flex items-center justify-center bg-blue-500 text-white rounded-full p-0.5 size-4 shadow-sm z-10">
                      <ThumbsUp className="size-2.5 fill-white text-white" />
                    </span>
                    <span className="flex items-center justify-center bg-red-500 text-white rounded-full p-0.5 size-4 shadow-sm -ml-1.5 z-20">
                      <Heart className="size-2.5 fill-white text-white" />
                    </span>
                  </span>
                  <span className="font-normal text-muted-foreground">
                    {post.likes}
                  </span>
                </div>

                <div className="flex items-center gap-2 text-muted-foreground">
                  <span>{commentsList.length} bình luận</span>
                  <span>•</span>
                  <span>{post.sharesCount} lượt chia sẻ</span>
                </div>
              </div>

              <div className="border-b border-border/50" />

              {/* ── Facebook Actions ── */}
              <div className="flex items-center gap-1 text-xs select-none">
                {/* Thích Button */}
                <button
                  onClick={() => handleLike(post.id)}
                  className={`flex items-center justify-center gap-2 flex-1 py-1.5 rounded-lg hover:bg-card/45 transition-colors cursor-pointer font-semibold ${
                    isLiked ? "text-blue-500" : "text-muted hover:text-foreground"
                  }`}
                >
                  <ThumbsUp className={`size-4 ${isLiked ? "fill-blue-500 text-blue-500" : ""}`} />
                  <span>Thích</span>
                </button>

                {/* Bình luận Button */}
                <button
                  onClick={() => handleFocusCommentInput(post.id)}
                  className="flex items-center justify-center gap-2 flex-1 py-1.5 rounded-lg hover:bg-card/45 transition-colors cursor-pointer font-semibold text-muted hover:text-foreground"
                >
                  <MessageSquare className="size-4" />
                  <span>Bình luận</span>
                </button>

                {/* Chia sẻ Button */}
                <button
                  onClick={() => handleSharePost(post.id)}
                  className="flex items-center justify-center gap-2 flex-1 py-1.5 rounded-lg hover:bg-card/45 transition-colors cursor-pointer font-semibold text-muted hover:text-foreground"
                >
                  <Share2 className="size-4" />
                  <span>Chia sẻ</span>
                </button>
              </div>

              <div className="border-b border-border/50" />

              {/* ── Facebook Comments Section ── */}
              <div className="space-y-3 pt-1">
                {/* Toggle to see more comments */}
                {hasManyComments && !showingAll && (
                  <button
                    onClick={() => setShowAllComments({ ...showAllComments, [post.id]: true })}
                    className="text-[11px] text-muted-foreground hover:text-accent font-semibold hover:underline block text-left pt-0.5 select-none cursor-pointer"
                  >
                    Xem thêm {commentsList.length - 3} bình luận
                  </button>
                )}

                {/* Comments list */}
                {visibleComments.length > 0 && (
                  <div className="space-y-3 max-h-[300px] overflow-y-auto pr-1">
                    {visibleComments.map((comment) => (
                      <div key={comment.id} className="flex gap-2 text-xs items-start">
                        {/* Comment Avatar */}
                        <div className="w-8 h-8 rounded-full bg-accent/10 border border-border flex items-center justify-center text-accent font-semibold text-xs shrink-0 select-none overflow-hidden">
                          {comment.authorAvatar ? (
                            // eslint-disable-next-line @next/next/no-img-element
                            <img src={comment.authorAvatar} alt={comment.authorName} className="w-full h-full object-cover" />
                          ) : (
                            comment.authorName.substring(0, 1).toUpperCase()
                          )}
                        </div>

                        {/* Comment Bubble */}
                        <div className="flex flex-col gap-0.5 max-w-[85%]">
                          <div className="bg-card/60 border border-border/60 px-3 py-1.5 rounded-2xl">
                            <span className="font-semibold text-foreground block text-[11px] leading-tight">
                              {comment.authorName}
                            </span>
                            <p className="text-foreground font-normal leading-normal whitespace-pre-line break-words text-[11px] mt-0.5">
                              {comment.content}
                            </p>
                          </div>

                          {/* Comment Actions */}
                          <div className="flex items-center gap-3 text-[10px] text-muted-foreground font-semibold px-2 select-none">
                            <button className="hover:text-foreground cursor-pointer">Thích</button>
                            <span>·</span>
                            <button className="hover:text-foreground cursor-pointer">Phản hồi</button>
                            <span>·</span>
                            <span>{comment.date}</span>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}

                {/* Comment Input block */}
                <div className="flex gap-2 pt-1 items-center">
                  {/* User Avatar */}
                  <div className="w-8 h-8 rounded-full bg-accent/10 border border-border flex items-center justify-center text-accent font-semibold text-xs shrink-0 select-none overflow-hidden">
                    {user?.avatarUrl ? (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img src={user.avatarUrl} alt={user.fullName} className="w-full h-full object-cover" />
                    ) : (
                      user?.fullName.substring(0, 1).toUpperCase() || "K"
                    )}
                  </div>

                  {/* Input box */}
                  <div className="relative flex-1">
                    <input
                      ref={(el) => {
                        commentInputRefs.current[post.id] = el;
                      }}
                      type="text"
                      placeholder={user ? `Bình luận dưới tên ${user.fullName}...` : "Viết bình luận công khai..."}
                      value={commentInputs[post.id] || ""}
                      onChange={(e) => handleCommentInputChange(post.id, e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter") {
                          handleSubmitComment(post.id);
                        }
                      }}
                      className="w-full bg-card/40 border border-border rounded-full pl-4 pr-10 py-1.5 text-xs focus:outline-hidden focus:border-accent text-foreground font-outfit font-normal"
                    />
                    <button
                      onClick={() => handleSubmitComment(post.id)}
                      aria-label="Send comment"
                      className="absolute right-3 top-1/2 -translate-y-1/2 text-muted hover:text-accent cursor-pointer flex items-center justify-center border-none"
                    >
                      <Send className="size-3.5" />
                    </button>
                  </div>
                </div>
              </div>
            </Card>
          );
        })}
      </div>

      {/* ── Side Widget Column ── */}
      <div className="space-y-6">
        {/* Corporate Stats */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Company details
          </Typography>

          <div className="space-y-3 text-xs select-none font-normal">
            {workspaceDetails.companyType && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Company Type</span>
                <span className="font-medium text-foreground text-xs">{workspaceDetails.companyType}</span>
              </div>
            )}

            {workspaceDetails.companySize && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Company Size</span>
                <span className="font-medium text-foreground text-xs">
                  {workspaceDetails.companySize.toLowerCase().includes("employee") ||
                    workspaceDetails.companySize.toLowerCase().includes("nhân viên")
                    ? workspaceDetails.companySize
                    : `${workspaceDetails.companySize} employees`}
                </span>
              </div>
            )}

            {workspaceDetails.industryTags && workspaceDetails.industryTags.length > 0 && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Primary Focus</span>
                <span className="font-medium text-foreground text-xs">{workspaceDetails.industryTags[0]}</span>
              </div>
            )}

            {workspaceDetails.website && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Website</span>
                <a
                  href={workspaceDetails.website}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="font-medium text-accent hover:underline break-all text-xs"
                >
                  {workspaceDetails.website.replace("https://", "").replace("http://", "")}
                </a>
              </div>
            )}

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Headquarters</span>
              <span className="font-medium text-foreground text-xs">
                {workspaceDetails.city
                  ? workspaceDetails.city.toLowerCase().includes("vietnam") ||
                    workspaceDetails.city.toLowerCase().includes("việt nam")
                    ? workspaceDetails.city
                    : `${workspaceDetails.city}, Vietnam`
                  : workspaceDetails.location || "Not specified"}
              </span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Branch Offices</span>
              <span className="font-medium text-foreground text-xs">{workspaceDetails.branchCount || 0} branches</span>
            </div>

            <div>
              <span className="text-[9px] text-muted-foreground uppercase block">Founded</span>
              <span className="font-medium text-foreground text-xs">{workspaceDetails.founded || "Not specified"}</span>
            </div>

            {workspaceDetails.taxCode && (
              <div>
                <span className="text-[9px] text-muted-foreground uppercase block">Tax Registered Code</span>
                <span className="font-medium text-foreground text-xs font-mono">{workspaceDetails.taxCode}</span>
              </div>
            )}
          </div>
        </Card>

        {/* Social Links */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Social Coordinates
          </Typography>
          <div className="flex flex-col gap-2 font-normal">
            {workspaceDetails.linkedinUrl && (
              <a
                href={workspaceDetails.linkedinUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>LinkedIn</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.facebookUrl && (
              <a
                href={workspaceDetails.facebookUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Facebook</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.twitterUrl && (
              <a
                href={workspaceDetails.twitterUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Twitter / X</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {workspaceDetails.website && (
              <a
                href={workspaceDetails.website}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between px-3 py-2 rounded-lg border border-border bg-card/5 hover:bg-card/15 transition-colors text-xs font-medium text-muted hover:text-foreground"
              >
                <span>Website Portal</span>
                <span className="text-[9px] text-accent uppercase">Visit</span>
              </a>
            )}
            {!workspaceDetails.linkedinUrl &&
              !workspaceDetails.facebookUrl &&
              !workspaceDetails.twitterUrl &&
              !workspaceDetails.website && (
                <span className="text-xs text-muted font-normal italic">No social coordinates specified.</span>
              )}
          </div>
        </Card>

        {/* Office Location */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-4">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Office Location
          </Typography>
          <div className="space-y-3 font-normal">
            <div className="text-xs text-foreground">
              <span>
                {workspaceDetails.detailAddress
                  ? `${workspaceDetails.detailAddress}, ${workspaceDetails.city || ""}`
                  : workspaceDetails.city || workspaceDetails.location || "No office locations registered."}
              </span>
            </div>
            {workspaceDetails.googleMapsEmbedUrl ? (
              <div className="h-48 rounded-xl overflow-hidden border border-border/80">
                <iframe
                  src={workspaceDetails.googleMapsEmbedUrl}
                  width="100%"
                  height="100%"
                  style={{ border: 0 }}
                  allowFullScreen={false}
                  loading="lazy"
                  title="Google Maps Location"
                />
              </div>
            ) : (
              <div className="h-28 border border-dashed border-border rounded-xl bg-surface-secondary/40 flex flex-col items-center justify-center text-muted select-none">
                <span className="text-xs font-medium italic">No interactive map location specified.</span>
              </div>
            )}
          </div>
        </Card>

        {/* Verification Highlights */}
        <Card className="p-5 bg-surface border border-border rounded-xl space-y-3">
          <Typography type="h4" className="font-semibold text-foreground text-xs uppercase tracking-wider block">
            Verification Badging
          </Typography>
          <Typography type="body-xs" className="text-muted leading-relaxed font-normal text-xs">
            This workspace holds a Level 3 Domain & Ownership verification status. All corporate information has been
            cryptographic-hashed and signed by CVerify Authorities.
          </Typography>
          <div className="pt-1 select-none font-normal text-xs text-muted-foreground space-y-1.5">
            <div className="flex items-center gap-1.5">
              <span>•</span>
              Legal Authority Verified
            </div>
            <div className="flex items-center gap-1.5">
              <span>•</span>
              Representative Signature Matches
            </div>
          </div>
        </Card>
      </div>

      {/* ── Scoped Form Drawer Modal Dialog for Announcement/Post Creation ── */}
      {showCreatePostModal && (
        <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-xs flex items-center justify-center p-4">
          <div className="bg-surface border border-border w-full max-w-xl rounded-xl shadow-2xl overflow-hidden font-outfit select-none flex flex-col max-h-[90vh]">
            {/* Modal Header */}
            <div className="p-4 border-b border-border flex items-center justify-between bg-card/10">
              <span className="font-semibold text-sm text-foreground">Write an announcement or share company updates...</span>
              <button
                onClick={() => setShowCreatePostModal(false)}
                className="p-1 rounded-full hover:bg-card/50 text-muted hover:text-foreground cursor-pointer border-none"
              >
                <X className="size-4" />
              </button>
            </div>

            {/* Modal Body */}
            <form onSubmit={handleCreatePostSubmit} className="p-6 overflow-y-auto space-y-4 text-xs font-normal">
              {/* Content Textarea */}
              <div className="space-y-1">
                <label className="text-[10px] text-muted uppercase font-semibold">Nội dung thông báo *</label>
                <textarea
                  required
                  rows={5}
                  placeholder="Nhập nội dung thông báo hoặc cập nhật công ty..."
                  value={newPostContent}
                  onChange={(e) => setNewPostContent(e.target.value)}
                  className="w-full bg-card border border-border rounded-lg px-3 py-2 text-xs text-foreground focus:outline-hidden focus:border-accent resize-none font-outfit"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                {/* Category Selection */}
                <div className="space-y-1">
                  <label className="text-[10px] text-muted uppercase font-semibold">Phân loại / Category</label>
                  <select
                    value={newPostCategory}
                    onChange={(e) => setNewPostCategory(e.target.value as any)}
                    className="w-full bg-card border border-border rounded-lg px-3 py-2 text-xs text-foreground focus:outline-hidden focus:border-accent cursor-pointer"
                  >
                    <option value="Announcement">Announcement</option>
                    <option value="Engineering">Engineering</option>
                    <option value="Recruitment">Recruitment</option>
                  </select>
                </div>

                {/* Author Display (Read Only or Prefilled) */}
                <div className="space-y-1">
                  <label className="text-[10px] text-muted uppercase font-semibold">Người đăng</label>
                  <input
                    type="text"
                    disabled
                    value={user?.fullName || "Manager"}
                    className="w-full bg-card/50 border border-border rounded-lg px-3 py-2 text-xs text-muted-foreground cursor-not-allowed"
                  />
                </div>
              </div>

              {/* Attachment Images */}
              <div className="space-y-1">
                <label className="text-[10px] text-muted uppercase font-semibold">
                  Hình ảnh đính kèm (Nhập URL hình ảnh, phân tách bằng dấu phẩy)
                </label>
                <input
                  type="text"
                  placeholder="Ví dụ: https://images.unsplash.com/..., https://images.unsplash.com/..."
                  value={newPostImages}
                  onChange={(e) => setNewPostImages(e.target.value)}
                  className="w-full bg-card border border-border rounded-lg px-3 py-2 text-xs text-foreground focus:outline-hidden focus:border-accent"
                />
                <span className="text-[10px] text-muted-foreground block">
                  Bạn có thể nhập một hoặc nhiều URL ảnh cách nhau bởi dấu phẩy để hiển thị dưới dạng lưới ảnh Facebook.
                </span>
              </div>

              {/* Modal Footer / Actions */}
              <div className="flex justify-end gap-2 pt-4 border-t border-border/40">
                <button
                  type="button"
                  onClick={() => setShowCreatePostModal(false)}
                  className="px-4 py-2 rounded-lg border border-border text-muted hover:text-foreground font-semibold hover:bg-card/50 transition-colors cursor-pointer text-xs"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 rounded-lg bg-accent text-background hover:bg-accent/90 transition-colors font-semibold cursor-pointer text-xs border-none"
                >
                  Post Announcement
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
